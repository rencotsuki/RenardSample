using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace SignageHADO
{
    using Sqlite;

    public class SystemLogDB : TemplateDB
    {
        public const string TemplateFileName = "SystemLogs";

        public const string StartupLogTableName = "StartupLog";
        public const string StatusLogTableName = "StatusLog";
        public const string ScoreLogTableName = "ScoreLog";

        /// <summary>起動ログ</summary>
        public class StartupLog : DataColumn
        {
            /// <summary>起動No</summary>
            public int No = 0;

            /// <summary>起動した時間</summary>
            public string StartupTime = String.Empty;

            /// <summary>アプリケーションバージョン</summary>
            public string ApplicationVersion = String.Empty;

            /// <summary>アプリのコミットハッシュ</summary>
            public string CommitHash = String.Empty;

            /// <summary>Assetsのコミットハッシュ</summary>
            public string AssetsCommitHash = String.Empty;

            /// <summary>ライセンスの期限</summary>
            public string LicenseDate = String.Empty;

            public static StartupLog New(DataRow dr = null)
            {
                var data = new StartupLog();
                data.Set(dr);
                return data;
            }

            protected override void OnSet(DataRow dr)
            {
                No = dr.Keys.Contains("No") ? (int)dr["No"] : 0;

                StartupTime = dr.Keys.Contains("StartupTime") ? (string)dr["StartupTime"] : string.Empty;

                ApplicationVersion = dr.Keys.Contains("ApplicationVersion") ? (string)dr["ApplicationVersion"] : string.Empty;

                CommitHash = dr.Keys.Contains("CommitHash") ? (string)dr["CommitHash"] : string.Empty;

                AssetsCommitHash = dr.Keys.Contains("AssetsCommitHash") ? (string)dr["AssetsCommitHash"] : string.Empty;

                LicenseDate = dr.Keys.Contains("LicenseDate") ? (string)dr["LicenseDate"] : string.Empty;
            }
        }

        /// <summary>状態ログ</summary>
        public class StatusLog : DataColumn
        {
            /// <summary>分析No</summary>
            public int No = 0;

            /// <summary>変化した時間</summary>
            public string ChangeTime = string.Empty;

            private string _strSignageStatus = string.Empty;
            /// <summary>サイネージ状態</summary>
            public SignageStatus SignageStatus = SignageStatus.None;

            private string _strGameStatus = string.Empty;
            /// <summary>ゲーム状態</summary>
            public GameStatus GameStatus = GameStatus.None;

            public static StatusLog New(DataRow dr = null)
            {
                var data = new StatusLog();
                data.Set(dr);
                return data;
            }

            protected override void OnSet(DataRow dr)
            {
                No = dr.Keys.Contains("No") ? (int)dr["No"] : 0;

                ChangeTime = dr.Keys.Contains("ChangeTime") ? (string)dr["ChangeTime"] : string.Empty;

                _strSignageStatus = dr.Keys.Contains("SignageStatus") ? (string)dr["SignageStatus"] : string.Empty;
                if (!Enum.TryParse(_strSignageStatus, out SignageStatus))
                    SignageStatus = SignageStatus.None;

                _strGameStatus = dr.Keys.Contains("GameStatus") ? (string)dr["GameStatus"] : string.Empty;
                if (!Enum.TryParse(_strGameStatus, out GameStatus))
                    GameStatus = GameStatus.None;
            }
        }

        /// <summary>スコアログ</summary>
        public class ScoreLog : DataColumn
        {
            /// <summary>ゲームNo</summary>
            public int No = 0;

            /// <summary>保存した時間</summary>
            public string SaveTime = string.Empty;

            /// <summary>左手発射数</summary>
            public int LeftShot = 0;

            /// <summary>右手発射数</summary>
            public int RightShot = 0;

            /// <summary>ヒット数</summary>
            public int Hit = 0;

            /// <summary>最大連続ヒット</summary>
            public int MaxCombo = 0;

            /// <summary>倒した的の数</summary>
            public int BreakEnemy = 0;

            /// <summary>獲得ポイント</summary>
            public int Point = 0;

            /// <summary>コンボボーナス点</summary>
            public int ComboBonus = 0;

            /// <summary>撃破数ボーナス点</summary>
            public int KillBonus = 0;

            /// <summary>総得点</summary>
            public int TotalPoint = 0;

            private string _strRank = string.Empty;
            /// <summary>獲得ランク</summary>
            public GameScoreRank Rank = GameScoreRank.C;

            public static ScoreLog New(DataRow dr = null)
            {
                var data = new ScoreLog();
                data.Set(dr);
                return data;
            }

            protected override void OnSet(DataRow dr)
            {
                No = dr.Keys.Contains("No") ? (int)dr["No"] : 0;

                SaveTime = dr.Keys.Contains("SaveTime") ? (string)dr["SaveTime"] : string.Empty;

                LeftShot = dr.Keys.Contains("LeftShot") ? (int)dr["LeftShot"] : 0;

                RightShot = dr.Keys.Contains("RightShot") ? (int)dr["RightShot"] : 0;

                Hit = dr.Keys.Contains("Hit") ? (int)dr["Hit"] : 0;

                MaxCombo = dr.Keys.Contains("MaxCombo") ? (int)dr["MaxCombo"] : 0;

                BreakEnemy = dr.Keys.Contains("BreakEnemy") ? (int)dr["BreakEnemy"] : 0;

                Point = dr.Keys.Contains("Point") ? (int)dr["Point"] : 0;

                ComboBonus = dr.Keys.Contains("ComboBonus") ? (int)dr["ComboBonus"] : 0;

                KillBonus = dr.Keys.Contains("KillBonus") ? (int)dr["KillBonus"] : 0;

                TotalPoint = dr.Keys.Contains("TotalPoint") ? (int)dr["TotalPoint"] : 0;

                _strRank = dr.Keys.Contains("Rank") ? (string)dr["Rank"] : string.Empty;
                if (!Enum.TryParse(_strRank, out Rank))
                    Rank = GameScoreRank.C;
            }
        }

        #region API

        public static async UniTask<StartupLog[]> GetStartupLogData(CancellationToken cancellationToken, string directoryPath, string dbFileName)
        {
            return await OnExecuteQueryCoroutine<StartupLog>(
                cancellationToken,
                directoryPath,
                dbFileName,
                CreateSelectQuery(StartupLogTableName, null));
        }

        public static async UniTask<StatusLog[]> GetStatusLogData(CancellationToken cancellationToken, string directoryPath, string dbFileName)
        {
            return await OnExecuteQueryCoroutine<StatusLog>(
                cancellationToken,
                directoryPath,
                dbFileName,
                CreateSelectQuery(StatusLogTableName, null));
        }

        public static async UniTask<ScoreLog[]> GetScoreLogData(CancellationToken cancellationToken, string directoryPath, string dbFileName)
        {
            return await OnExecuteQueryCoroutine<ScoreLog>(
                cancellationToken,
                directoryPath,
                dbFileName,
                CreateSelectQuery(ScoreLogTableName, null));
        }

        #endregion
    }
}
