using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;
using Renard;

namespace SignageHADO
{
    using Game;
    using Sqlite;

    public class SignageLogHandler : MonoBehaviourCustom
    {
        [Header("ファイル保管期間[日] ※0設定は消さない")]
        [SerializeField, Range(0, 120)] private int deleteOldDataDays = 30;
        [Header("ファイル書込みの間隔[秒] ※0設定は即時")]
        [SerializeField] private float fileWriteIntervalTime = 30f;
        [Header("トラッキングログ収集の間隔[秒]")]
        [SerializeField, Range(1f, 300f)] private float trackingLogIntervalTime = 15f;

        public bool Init { get; private set; } = false;

        protected DateTime logTimeNow => DateTime.Now;  // utcNowでなくNowにしておく
        private const string logDateTimeFormat = "yyyy/MM/dd HH:mm:ss";

        protected const int writeWaitTimeMilliseconds = 500;

        protected const float updateBatteryLevelIntervalTime = 60f;

        private const char fileSplitMoji = '_';
        private const string fileDateTimeFormat = "yyyyMMdd";

        protected string fileExtension => SqliteHandler.FileExtension;
        protected string templateFilePath => $"{Application.streamingAssetsPath}/Loggers";

        protected string outputFilePath { get; private set; } = string.Empty;

        protected string outputSystemLogFile { get; private set; } = string.Empty;
        protected string outputTrackingLogFile { get; private set; } = string.Empty;

        protected SignageManager signageManager => SignageManager.Singleton;
        protected SignageStatus signageStatus => signageManager != null ? signageManager.Status : SignageStatus.None;
        protected GameManager gameManager => GameManager.Singleton;
        protected GameStatus gameStatus => gameManager != null ? gameManager.Status : GameStatus.None;

        protected SqliteHandler sqliteHandler = null;

        protected int startupLogNo = 0;
        protected int statusLogNo = 0;
        protected int scoreLogNo = 0;
        protected int trackingLogNo = 0;

        protected bool fileWriting = false;
        protected StringBuilder systemLogScriptPool = new StringBuilder();
        protected StringBuilder trackingLogScriptPool = new StringBuilder();

        [HideInInspector] private int _index = 0;
        [HideInInspector] private IEnumerable<string> _files = null;
        [HideInInspector] private string _fileName = null;
        [HideInInspector] private string[] _split = null;
        [HideInInspector] private DateTime _fileDateTime = default;
        [HideInInspector] private TimeSpan _fileTimeSpan = default;
        [HideInInspector] private string _tmpScript = string.Empty;
        [HideInInspector] private StringBuilder _tmpStringBuilder = new StringBuilder();

        private CancellationTokenSource _onSetupToken = null;
        private CancellationTokenSource _onUpdateToken = null;
        private CancellationTokenSource _onFileWriteToken = null;

        private void OnDestroy()
        {
            OnDisposeSetup();
            OnDisposeUpdate();
            OnDisposeFileWrite();
        }

        private void OnApplicationQuit()
        {
            OnDisposeFileWrite();

            // 投げ捨て処理させる
            OnFileWriteAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private void OnDisposeSetup()
        {
            _onSetupToken?.Dispose();
            _onSetupToken = null;
        }

        public void Setup(string outputPath, string assetsCommitHash, string licenseDate)
        {
            outputFilePath = outputPath;

            OnDisposeSetup();
            _onSetupToken = new CancellationTokenSource();
            OnSetupAsync(_onSetupToken.Token, assetsCommitHash, licenseDate).Forget();
        }

        private async UniTask OnSetupAsync(CancellationToken token, string assetsCommitHash, string licenseDate)
        {
            Init = false;

            try
            {
                await UpdateSignageLogsAsync(token);

                // StartupLogの更新
                var sqliteScript = CreateQueryStartupLog(assetsCommitHash, licenseDate);
                await OnSaveSignageLogsAsync(token, outputSystemLogFile, sqliteScript, SystemLogDB.TemplateFileName);
                token.ThrowIfCancellationRequested();

                Observable.Merge(
                    this.ObserveEveryValueChanged(x => x.signageStatus).Select(x => true),
                    this.ObserveEveryValueChanged(x => x.gameStatus).Select(x => true))
                    .Subscribe(_ => OnChangeStatus(signageStatus, gameStatus))
                    .AddTo(this);

                gameManager.ReportGameResultSubject
                    .Subscribe(OnReportGameResult)
                    .AddTo(this);

                OnDisposeUpdate();
                _onUpdateToken = new CancellationTokenSource();
                OnUpdateAsync(_onUpdateToken.Token).Forget();

                Init = true;
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnSetupAsync", $"{ex.Message}");
            }
        }

        private void DeleteOldData(string outputFilePath)
        {
            // 0以下の日付指定の時は処理しない
            if (deleteOldDataDays <= 0)
                return;

            try
            {
                if (!Directory.Exists(outputFilePath))
                    throw new Exception($"not found output path. path={outputFilePath}");

                _files = Directory.EnumerateFiles(outputFilePath, $"*.{fileExtension}", SearchOption.TopDirectoryOnly);
                _fileName = string.Empty;

                for (_index = 0; _index < _files.Count(); _index++)
                {
                    _fileName = Path.GetFileNameWithoutExtension(_files.ElementAt(_index));
                    _split = _fileName.Split(fileSplitMoji);

                    if (_split.Length <= 1) continue;

                    _fileDateTime = DateTime.ParseExact(_split.Last(), fileDateTimeFormat, null);
                    _fileTimeSpan = logTimeNow - _fileDateTime;

                    if (_fileTimeSpan.TotalDays <= deleteOldDataDays) continue;

                    File.Delete(_files.ElementAt(_index));
                }
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "DeleteOldData", $"{ex.Message}");
            }
        }

        private void OnDisposeUpdate()
        {
            _onUpdateToken?.Dispose();
            _onUpdateToken = null;
        }

        private async UniTask OnUpdateAsync(CancellationToken token)
        {
            var fileWriteInterval = Time.realtimeSinceStartup;
            var trackingLogInterval = Time.realtimeSinceStartup;
            var updateBatteryInterval = Time.realtimeSinceStartup;
            var nowDay = logTimeNow.Day;
            var batteryLevel = BatteryInfo.Level;

            while (_onUpdateToken != null)
            {
                // 日付が変わったら更新
                if (nowDay != logTimeNow.Day)
                {
                    await UpdateSignageLogsAsync(token);
                    nowDay = logTimeNow.Day;
                }

                // TrackingLogの更新
                if (trackingLogIntervalTime <= 0 || Time.realtimeSinceStartup - trackingLogInterval >= trackingLogIntervalTime)
                {
                    _tmpScript = CreateQueryTrackingLog(batteryLevel);
                    if (_tmpScript != null && _tmpScript.Length > 0)
                        trackingLogScriptPool.Append(_tmpScript);

                    trackingLogInterval = Time.realtimeSinceStartup;
                }

                // ※ゲームパート、テスト中のゲーム時には処理しない
                if (signageStatus != SignageStatus.GamePart || (signageStatus != SignageStatus.TestLoop && gameStatus != GameStatus.Playing))
                {
                    // バッテリー更新
                    if (updateBatteryLevelIntervalTime <= 0 || Time.realtimeSinceStartup - updateBatteryInterval >= updateBatteryLevelIntervalTime)
                    {
                        batteryLevel = BatteryInfo.Level;
                        updateBatteryInterval = Time.realtimeSinceStartup;
                    }

                    // 書込み間隔
                    if (fileWriteIntervalTime <= 0 || Time.realtimeSinceStartup - fileWriteInterval >= fileWriteIntervalTime)
                    {
                        if (!fileWriting)
                        {
                            OnDisposeFileWrite();
                            _onFileWriteToken = new CancellationTokenSource();
                            OnFileWriteAsync(_onFileWriteToken.Token).Forget();

                            fileWriteInterval = Time.realtimeSinceStartup;
                        }
                    }
                }

                await UniTask.Yield(PlayerLoopTiming.Update, token);
                token.ThrowIfCancellationRequested();
            }
        }

        private async UniTask<SqliteHandler> OnCreateLoadSignageLogsAsync(CancellationToken token, string outputFilePath, string fileName, string templateFileName)
        {
            try
            {
                if (File.Exists($"{outputFilePath}/{fileName}.{fileExtension}"))
                    return new SqliteHandler(outputFilePath, fileName);
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnCreateLoadSignageLogsAsync", $"{ex.Message}");
            }

            try
            {
                if (!File.Exists($"{templateFilePath}/{templateFileName}.{fileExtension}"))
                    throw new Exception($"not found template file. path={templateFilePath}/{templateFileName}.{fileExtension}");

                if (!Directory.Exists(outputFilePath))
                    Directory.CreateDirectory(outputFilePath);

                File.Copy($"{templateFilePath}/{templateFileName}.{fileExtension}", $"{outputFilePath}/{fileName}.{fileExtension}");

                await UniTask.NextFrame(token);

                return new SqliteHandler(outputFilePath, fileName);
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnCreateLoadSignageLogsAsync", $"{ex.Message}");
                return null;
            }
        }

        private async UniTask UpdateSignageLogsAsync(CancellationToken token)
        {
            outputSystemLogFile = $"{SystemLogDB.TemplateFileName}{fileSplitMoji}{logTimeNow.ToString(fileDateTimeFormat)}";
            outputTrackingLogFile = $"{TrackingLogDB.TemplateFileName}{fileSplitMoji}{logTimeNow.ToString(fileDateTimeFormat)}";

            DeleteOldData(outputFilePath);

            var startupData = await SystemLogDB.GetStartupLogData(token, outputFilePath, outputSystemLogFile);
            startupLogNo = startupData != null && startupData.Length > 0 ? startupData.Length + 1 : 1;

            var statusData = await SystemLogDB.GetStatusLogData(token, outputFilePath, outputSystemLogFile);
            statusLogNo = statusData != null && statusData.Length > 0 ? statusData.Length + 1 : 1;

            var scoreData = await SystemLogDB.GetScoreLogData(token, outputFilePath, outputSystemLogFile);
            scoreLogNo = scoreData != null && scoreData.Length > 0 ? scoreData.Length + 1 : 1;

            var trackingData = await TrackingLogDB.GetTrackingLogData(token, outputFilePath, outputTrackingLogFile);
            trackingLogNo = trackingData != null && trackingData.Length > 0 ? trackingData.Length + 1 : 1;
        }

        private void OnDisposeFileWrite()
        {
            _onFileWriteToken?.Dispose();
            _onFileWriteToken = null;
            fileWriting = false;
        }

        private async UniTask OnFileWriteAsync(CancellationToken token)
        {
            try
            {
                fileWriting = true;

                try
                {
                    if (systemLogScriptPool.Length > 0)
                    {
                        // ファイル書込み
                        await OnSaveSignageLogsAsync(token, outputSystemLogFile, systemLogScriptPool.ToString(), SystemLogDB.TemplateFileName);
                        token.ThrowIfCancellationRequested();

                        systemLogScriptPool.Length = 0;
                    }
                }
                catch (Exception ex)
                {
                    Log(DebugerLogType.Info, "OnFileWriteAsync", $"{ex.Message}");

                    if (token.IsCancellationRequested)
                        return;
                }

                try
                {
                    if (trackingLogScriptPool.Length > 0)
                    {
                        // ファイル書込み
                        await OnSaveSignageLogsAsync(token, outputTrackingLogFile, trackingLogScriptPool.ToString(), TrackingLogDB.TemplateFileName);
                        token.ThrowIfCancellationRequested();

                        trackingLogScriptPool.Length = 0;
                    }
                }
                catch (Exception ex)
                {
                    Log(DebugerLogType.Info, "OnFileWriteAsync", $"{ex.Message}");

                    if (token.IsCancellationRequested)
                        return;
                }
            }
            finally
            {
                fileWriting = false;
            }
        }

        private async UniTask OnSaveSignageLogsAsync(CancellationToken token, string outputFileName, string querys, string templateFileName)
        {
            if (querys == null || querys.Length <= 0)
                return;

            try
            {
                sqliteHandler = await OnCreateLoadSignageLogsAsync(token, outputFilePath, outputFileName, templateFileName);
                if (sqliteHandler != null)
                {
                    var waitTimeoutToken = new CancellationTokenSource();
                    waitTimeoutToken.CancelAfterSlim(TimeSpan.FromMilliseconds(writeWaitTimeMilliseconds));
                    var waitLinkedToken = CancellationTokenSource.CreateLinkedTokenSource(token, waitTimeoutToken.Token);

                    await UniTask.SwitchToMainThread();
                    sqliteHandler.ExecuteScript(querys);
                }
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnSaveSignageLogsAsync", $"{ex.Message}");
            }
            finally
            {
                sqliteHandler?.Close();
                sqliteHandler = null;
            }
        }

        private string CreateQueryStartupLog(string assetsCommitHash, string licenseDate)
        {
            try
            {
                var appVersion = ApplicationVersionAsset.Load();

                _tmpStringBuilder.Length = 0;

                /*
                 * INSERT INTO StartupLog
                 *      VALUES('Id',
                 *             No, 'StartupTime', 'ApplicationVersion', 'CommitHash', 'AssetsCommitHash', 'LicenseDate'
                 *             'Other');
                 */

                _tmpStringBuilder.Append($"INSERT INTO {SystemLogDB.StartupLogTableName}");
                _tmpStringBuilder.Append(" VALUES (");
                _tmpStringBuilder.Append($"'{startupLogNo}'");

                _tmpStringBuilder.Append($", {startupLogNo}");
                _tmpStringBuilder.Append($", '{logTimeNow.ToString(logDateTimeFormat)}'");
                _tmpStringBuilder.Append($", '{(appVersion == null || string.IsNullOrEmpty(appVersion.Version) ? "---" : $"{appVersion.Version}({appVersion.BuildNumber})")}'");
                _tmpStringBuilder.Append($", '{(appVersion == null || string.IsNullOrEmpty(appVersion.CommitHash) ? "---" : appVersion.CommitHash)}'");
                _tmpStringBuilder.Append($", '{(string.IsNullOrEmpty(assetsCommitHash) ? "---" : assetsCommitHash)}'");
                _tmpStringBuilder.Append($", '{(string.IsNullOrEmpty(licenseDate) ? "---" : licenseDate)}'");

                _tmpStringBuilder.Append($",''");
                _tmpStringBuilder.Append(");");

                startupLogNo += 1;

                return _tmpStringBuilder.ToString();
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "CreateQueryStartupLog", $"{ex.Message}");
                return null;
            }
        }

        private string CreateQueryStatusLog(SignageStatus signageStatus, GameStatus gameStatus)
        {
            try
            {
                _tmpStringBuilder.Length = 0;

                /*
                 * INSERT INTO StatusLog
                 *      VALUES('Id',
                 *             No, 'ChangeTime', 'SignageStatus', 'GameStatus',
                 *             'Other');
                 */

                _tmpStringBuilder.Append($"INSERT INTO {SystemLogDB.StatusLogTableName}");
                _tmpStringBuilder.Append(" VALUES (");
                _tmpStringBuilder.Append($"'{statusLogNo}'");

                _tmpStringBuilder.Append($", {statusLogNo}");
                _tmpStringBuilder.Append($", '{logTimeNow.ToString(logDateTimeFormat)}'");
                _tmpStringBuilder.Append($", '{signageStatus}'");
                _tmpStringBuilder.Append($", '{gameStatus}'");

                _tmpStringBuilder.Append($",''");
                _tmpStringBuilder.Append(");");

                statusLogNo += 1;

                return _tmpStringBuilder.ToString();
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "CreateQueryStatusLog", $"{ex.Message}");
                return null;
            }
        }

        private string CreateQueryScoreLog(GameScore score)
        {
            try
            {
                _tmpStringBuilder.Length = 0;

                /*
                 * INSERT INTO ScoreLog
                 *      VALUES('Id',
                 *             No, 'SaveTime', LeftShot, RightShot, Hit, MaxCombo, BreakEnemy,
                 *             Point, ComboBonus, KillBonus, TotalPoint, 'Rank',
                 *             'Other');
                 */

                _tmpStringBuilder.Append($"INSERT INTO {SystemLogDB.ScoreLogTableName}");
                _tmpStringBuilder.Append(" VALUES (");
                _tmpStringBuilder.Append($"'{scoreLogNo}'");

                _tmpStringBuilder.Append($", {scoreLogNo}");
                _tmpStringBuilder.Append($", '{logTimeNow.ToString(logDateTimeFormat)}'");
                _tmpStringBuilder.Append($", {score.LeftShot}");
                _tmpStringBuilder.Append($", {score.RightShot}");
                _tmpStringBuilder.Append($", {score.Hit}");
                _tmpStringBuilder.Append($", {score.MaxCombo}");
                _tmpStringBuilder.Append($", {score.BreakEnemy}");
                _tmpStringBuilder.Append($", {score.Point}");
                _tmpStringBuilder.Append($", {score.ComboBonus}");
                _tmpStringBuilder.Append($", {score.KillBonus}");
                _tmpStringBuilder.Append($", {score.TotalPoint}");
                _tmpStringBuilder.Append($", '{score.Rank}'");

                _tmpStringBuilder.Append($",''");
                _tmpStringBuilder.Append(");");

                scoreLogNo += 1;

                return _tmpStringBuilder.ToString();
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "CreateQueryScoreLog", $"{ex.Message}");
                return null;
            }
        }

        private string CreateQueryTrackingLog(float batteryLevel)
        {
            try
            {
                _tmpStringBuilder.Length = 0;

                /*
                 * INSERT INTO TrackingLog
                 *      VALUES('Id',
                 *             No, 'TrackingTime', BatteryPower, CameraDevicePitch, CameraDeviceYaw, CameraDeviceRoll,
                 *             PersonCount, HeadTracking, LeftHandTracking, RightHandTracking,
                 *             'Other');
                 */

                _tmpStringBuilder.Append($"INSERT INTO {TrackingLogDB.TrackingLogTableName}");
                _tmpStringBuilder.Append(" VALUES (");
                _tmpStringBuilder.Append($"'{trackingLogNo}'");

                _tmpStringBuilder.Append($", {trackingLogNo}");
                _tmpStringBuilder.Append($", '{logTimeNow.ToString(logDateTimeFormat)}'");
                _tmpStringBuilder.Append($", {batteryLevel:0.00}");
                _tmpStringBuilder.Append($", {TrackingHandler.CameraDeviceEulerAngles.x}");
                _tmpStringBuilder.Append($", {TrackingHandler.CameraDeviceEulerAngles.y}");
                _tmpStringBuilder.Append($", {TrackingHandler.CameraDeviceEulerAngles.z}");
                _tmpStringBuilder.Append($", {TrackingHandler.DetectionTargetCount}");
                _tmpStringBuilder.Append($", {(TrackingHandler.IsHeadTracking ? 1 : 0)}");
                _tmpStringBuilder.Append($", {(TrackingHandler.IsLeftHandTracking ? 1 : 0)}");
                _tmpStringBuilder.Append($", {(TrackingHandler.IsRightHandTracking ? 1 : 0)}");

                _tmpStringBuilder.Append($",''");
                _tmpStringBuilder.Append(");");

                trackingLogNo += 1;

                return _tmpStringBuilder.ToString();
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "CreateQueryTrackingLog", $"{ex.Message}");
                return null;
            }
        }

        private void OnChangeStatus(SignageStatus signageStatus, GameStatus gameStatus)
        {
            _tmpScript = CreateQueryStatusLog(signageStatus, gameStatus);

            if (_tmpScript != null && _tmpScript.Length > 0)
                systemLogScriptPool.Append(_tmpScript);
        }

        private void OnReportGameResult(GameScore score)
        {
            _tmpScript = CreateQueryScoreLog(score);

            if (_tmpScript != null && _tmpScript.Length > 0)
                systemLogScriptPool.Append(_tmpScript);
        }
    }
}
