/*
 * Mediapipe - Bootstrapの改修
 */

using System.Collections;
using UnityEngine;
using Mediapipe;
using Mediapipe.Unity;
using Mediapipe.Unity.Sample;

namespace SignageHADO.Tracking
{
    public class MediapipeBootstrap : MonoBehaviourCustom
    {
        [SerializeField] private MediapipeAppSettings _appSettings = null;

        public InferenceMode inferenceMode { get; private set; }
        public bool isFinished { get; private set; }
        private bool _isGlogInitialized;

        private void OnEnable()
        {
            var _ = StartCoroutine(Init());
        }

        private IEnumerator Init()
        {
            Log(DebugerLogType.Info, "Init", "The configuration for the sample app can be modified using AppSettings.asset.");
#if !DEBUG && !DEVELOPMENT_BUILD
            Log(DebugerLogType.Info, "Init", "Logging for the MediaPipeUnityPlugin will be suppressed. To enable logging, please check the 'Development Build' option and build.");
#endif

            Mediapipe.Logger.MinLogLevel = _appSettings.logLevel;

            Protobuf.SetLogHandler(Protobuf.DefaultLogHandler);

            Log(DebugerLogType.Info, "Init", "Setting global flags...");
            _appSettings.ResetGlogFlags();
            Glog.Initialize("MediaPipeUnityPlugin");
            _isGlogInitialized = true;

            Log(DebugerLogType.Info, "Init", "Initializing AssetLoader...");
            switch (_appSettings.assetLoaderType)
            {
                case MediapipeAppSettings.AssetLoaderType.AssetBundle:
                    {
                        AssetLoader.Provide(new AssetBundleResourceManager("mediapipe"));
                        break;
                    }
                case MediapipeAppSettings.AssetLoaderType.StreamingAssets:
                    {
                        AssetLoader.Provide(new StreamingAssetsResourceManager("mediapipe"));
                        break;
                    }
                case MediapipeAppSettings.AssetLoaderType.Local:
                    {
#if UNITY_EDITOR
                        AssetLoader.Provide(new LocalResourceManager());
                        break;
#else
                        Log(DebugerLogType.Info, "Init", "LocalResourceManager is only supported on UnityEditor." +
                          "To avoid this error, consider switching to the StreamingAssetsResourceManager and copying the required resources under StreamingAssets, for example.");
                        yield break;
#endif
                    }
                default:
                    {
                        Log(DebugerLogType.Info, "Init", $"AssetLoaderType is unknown: {_appSettings.assetLoaderType}");
                        yield break;
                    }
            }

            DecideInferenceMode();
            if (inferenceMode == InferenceMode.GPU)
            {
                Log(DebugerLogType.Info, "Init", "Initializing GPU resources...");
                yield return GpuManager.Initialize();

                if (!GpuManager.IsInitialized)
                {
                    Log(DebugerLogType.Info, "Init", "If your native library is built for CPU, change 'Preferable Inference Mode' to CPU from the Inspector Window for AppSettings");
                }
            }

            Log(DebugerLogType.Info, "Init", "Preparing ImageSource...");

            ImageSourceProvider.Initialize(_appSettings.BuildWebCamSource(), _appSettings.BuildStaticImageSource());
            ImageSourceProvider.Switch(_appSettings.defaultImageSource);

            isFinished = true;
        }

        private void DecideInferenceMode()
        {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_WIN
            if (_appSettings.preferableInferenceMode == InferenceMode.GPU)
            {
                Log(DebugerLogType.Info, "DecideInferenceMode", "Current platform does not support GPU inference mode, so falling back to CPU mode");
            }
            inferenceMode = InferenceMode.CPU;
#else
            inferenceMode = _appSettings.preferableInferenceMode;
#endif
        }

        private void OnApplicationQuit()
        {
            GpuManager.Shutdown();

            if (_isGlogInitialized)
            {
                Glog.Shutdown();
            }

            Protobuf.ResetLogHandler();
        }
    }
}
