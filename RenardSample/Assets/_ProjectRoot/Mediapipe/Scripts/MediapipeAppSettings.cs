/*
 * Mediapipe - AppSettingsの改修
 */

using System;
using UnityEngine;
using Mediapipe;
using Mediapipe.Unity;

namespace SignageHADO.Tracking
{
    [Serializable]
    public class MediapipeAppSettings : ScriptableObject
    {
        [Serializable]
        public enum AssetLoaderType
        {
            StreamingAssets,
            AssetBundle,
            Local,
        }

        [SerializeField] private ImageSourceType _defaultImageSource;
        [SerializeField] private InferenceMode _preferableInferenceMode;
        [SerializeField] private AssetLoaderType _assetLoaderType;
        [SerializeField] private Mediapipe.Logger.LogLevel _logLevel = Mediapipe.Logger.LogLevel.Debug;

        [Header("Glog settings")]
        [SerializeField] private int _glogMinloglevel = Glog.Minloglevel;
        [SerializeField] private int _glogStderrthreshold = Glog.Stderrthreshold;
        [SerializeField] private int _glogV = Glog.V;

        [Header("WebCam Source")]
        [Tooltip("For the default resolution, the one whose width is closest to this value will be chosen")]

        [SerializeField] private int _preferredDefaultWebCamWidth = 1280;
        [SerializeField]
        private ImageSource.ResolutionStruct[] _defaultAvailableWebCamResolutions = new ImageSource.ResolutionStruct[]
        {
            new ImageSource.ResolutionStruct(176, 144, 30),
            new ImageSource.ResolutionStruct(320, 240, 30),
            new ImageSource.ResolutionStruct(424, 240, 30),
            new ImageSource.ResolutionStruct(640, 360, 30),
            new ImageSource.ResolutionStruct(640, 480, 30),
            new ImageSource.ResolutionStruct(848, 480, 30),
            new ImageSource.ResolutionStruct(960, 540, 30),
            new ImageSource.ResolutionStruct(1280, 720, 30),
            new ImageSource.ResolutionStruct(1600, 896, 30),
            new ImageSource.ResolutionStruct(1920, 1080, 30),
        };

        [Header("Static Image Source")]
        [SerializeField] private Texture[] _availableStaticImageSources;
        [SerializeField]
        private ImageSource.ResolutionStruct[] _defaultAvailableStaticImageResolutions = new ImageSource.ResolutionStruct[]
        {
            new ImageSource.ResolutionStruct(512, 512, 0),
            new ImageSource.ResolutionStruct(640, 480, 0),
            new ImageSource.ResolutionStruct(1280, 720, 0),
        };

        public ImageSourceType defaultImageSource => _defaultImageSource;
        public InferenceMode preferableInferenceMode => _preferableInferenceMode;
        public AssetLoaderType assetLoaderType
        {
            get => _assetLoaderType;
            set => _assetLoaderType = value;
        }
        public Mediapipe.Logger.LogLevel logLevel => _logLevel;

        public void ResetGlogFlags()
        {
            Glog.Logtostderr = true;
            Glog.Minloglevel = _glogMinloglevel;
            Glog.Stderrthreshold = _glogStderrthreshold;
            Glog.V = _glogV;
        }

        public WebCamSource BuildWebCamSource() => new WebCamSource(
          _preferredDefaultWebCamWidth,
          _defaultAvailableWebCamResolutions
        );

        public StaticImageSource BuildStaticImageSource() => new StaticImageSource(
          _availableStaticImageSources,
          _defaultAvailableStaticImageResolutions
        );
    }
}
