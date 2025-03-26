using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace SignageHADO
{
    public class LauncherTools : MonoBehaviourCustom
    {
        public const string TargetSceneNameCreateLicense = "CreateLicenseApp.unity";
        public const string TargetSceneNameDownloader = "DownloaderApp.unity";

        private CancellationTokenSource _startupToken = null;

        private void Start()
        {
            Startup();
        }

        private void OnDestroy()
        {
            OnDisposeStartup();
        }

        private void OnDisposeStartup()
        {
            _startupToken?.Dispose();
            _startupToken = null;
        }

        private void Startup()
        {
            OnDisposeStartup();
            _startupToken = new CancellationTokenSource();
            OnStartupAsync(_startupToken.Token).Forget();
        }

        private async UniTask OnStartupAsync(CancellationToken token)
        {
            try
            {
                // スプラッシュ表示が完了しているか確認する
                await UniTask.WaitWhile(() => !SplashScreen.isFinished, cancellationToken: token);
                token.ThrowIfCancellationRequested();

#if UNITY_EDITOR
                // エディタで直接LauncherToolsを呼んだ時の導線
                await EditorLoadToolSceneAsync(token, Launcher.IsCreateLicenseMode, Launcher.IsDownloaderMode);
                token.ThrowIfCancellationRequested();
#else
                // インデクスで呼び出すのに注意！
                await SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
                token.ThrowIfCancellationRequested();
#endif
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnStartupAsync", $"{ex.Message}");

                if (!token.IsCancellationRequested)
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                }
            }
        }

        public static async UniTask<bool> EditorLoadToolSceneAsync(CancellationToken token, bool createLicense, bool downloader)
        {
            try
            {
#if UNITY_EDITOR
                var targetSceneName = createLicense ? TargetSceneNameCreateLicense : downloader ? TargetSceneNameDownloader : string.Empty;
                var scenePath = string.Empty;

                foreach (var scene in EditorBuildSettings.scenes)
                {
                    if (Path.GetFileName(scene.path) == targetSceneName)
                    {
                        scenePath = scene.path;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(scenePath))
                    throw new Exception($"not found scene. sceneName={targetSceneName}");

                // 非アクティブなシーンの読込み
                await EditorSceneManager.LoadSceneAsyncInPlayMode(scenePath, new LoadSceneParameters(LoadSceneMode.Single));
                token.ThrowIfCancellationRequested();

                return true;
#endif
            }
            catch
            {
                // 何もしない
            }
            return false;
        }
    }
}
