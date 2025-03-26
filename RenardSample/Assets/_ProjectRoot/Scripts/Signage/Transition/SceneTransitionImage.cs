using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SignageHADO
{
    [Serializable]
    public class SceneTransitionImage : Graphic
    {
        [SerializeField] private Texture _maskTexture = default;
        public Texture MaskTexture
        {
            get { return _maskTexture; }
            set
            {
                _maskTexture = value;
                material?.SetTexture("_MaskTex", _maskTexture);
            }
        }

        [SerializeField, Range(0, 1)] private float _cutoutRange = 0f;
        public float CutoutRange
        {
            get { return _cutoutRange; }
            set
            {
                try
                {
                    _cutoutRange = value;
                    material?.SetFloat("_Range", 1 - _cutoutRange);
                    enabled = (_cutoutRange > 0);
                }
                catch
                {
                    // 何もしない
                }
            }
        }

        public Color GraphicColor
        {
            get { return color; }
            set
            {
                color = value;
                material?.SetColor("_Color", color);
            }
        }

        private TransitionType _materialType = TransitionType.Cutout;
        public TransitionType MaterialType
        {
            get { return _materialType; }
            protected set
            {
                _materialType = value;
                switch (_materialType)
                {
                    case TransitionType.Cutout:
                        material = new Material(_fadeCutout);
                        break;

                    case TransitionType.Alpha:
                        material = new Material(_fadeAlpha);
                        break;

                    case TransitionType.Mask:
                        material = new Material(_fadeMask);
                        break;

                    default:
                        break;
                }
                MaskTexture = _maskTexture;
            }
        }

        [SerializeField] private Material _fadeCutout = default;
        [SerializeField] private Material _fadeAlpha = default;
        [SerializeField] private Material _fadeMask = default;

        public TransitionFadeMode CurrentMode { get; protected set; } = TransitionFadeMode.FadeOut;

        private Material _instMaterial = default;
        private AnimationCurve _fadeCurve = null;
        private float _fadeRate = 0f;
        private float _endTime = 0f;

        protected override void Awake()
        {
            base.Awake();

            SetupMaterial();
            OnReset();
        }

        private void SetupMaterial()
        {
            if (material != null)
            {
                _instMaterial = new Material(material);
                _instMaterial.CopyPropertiesFromMaterial(material);
            }
            else if (_fadeCutout != null)
            {
                _instMaterial = new Material(_fadeCutout);
                _instMaterial.CopyPropertiesFromMaterial(_fadeCutout);
            }
            else if (_fadeAlpha != null)
            {
                _instMaterial = new Material(_fadeAlpha);
                _instMaterial.CopyPropertiesFromMaterial(_fadeAlpha);
            }
            else if (_fadeMask != null)
            {
                _instMaterial = new Material(_fadeMask);
                _instMaterial.CopyPropertiesFromMaterial(_fadeMask);
            }

            material = _instMaterial;
        }

        public new void Reset()
        {
#if UNITY_EDITOR
            base.Reset();
#endif
            OnReset();
        }

        protected void OnReset()
        {
            GraphicColor = Color.black;
            CutoutRange = CurrentMode == TransitionFadeMode.FadeIn ? 1f : 0f;
            _fadeRate = 0f;
        }

        public async UniTask PlayFade(CancellationToken token, Color graphicColor, TransitionParameter parameter, float fadeTime, float delayTime)
        {
            OnReset();

            MaterialType = parameter.Type;
            GraphicColor = graphicColor;
            CurrentMode = parameter.Mode;
            _fadeCurve = parameter.Curve;

            await FadeCoroutine(token, fadeTime, delayTime);
            token.ThrowIfCancellationRequested();

            CutoutRange = CurrentMode == TransitionFadeMode.FadeIn ? 1f : 0f;
            GraphicColor = graphicColor;
        }

        private float GetEvaluateValue(float fadeRate)
        {
            return CurrentMode == TransitionFadeMode.FadeIn ? 1 - _fadeCurve.Evaluate(fadeRate) : _fadeCurve.Evaluate(fadeRate);
        }

        private async UniTask FadeCoroutine(CancellationToken token, float fadeTime, float delayTime)
        {
            _fadeRate = 1f;
            CutoutRange = GetEvaluateValue(_fadeRate);

            if (delayTime > 0f)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(delayTime), cancellationToken: token);
                token.ThrowIfCancellationRequested();
            }

            if (fadeTime > 0f)
            {
                _endTime = Time.timeSinceLevelLoad + fadeTime;

                while (Time.timeSinceLevelLoad <= _endTime)
                {
                    await UniTask.Yield(PlayerLoopTiming.LastUpdate, cancellationToken: token);
                    token.ThrowIfCancellationRequested();

                    _fadeRate = (_endTime - Time.timeSinceLevelLoad) / fadeTime;
                    CutoutRange = GetEvaluateValue(_fadeRate);
                }
            }

            _fadeRate = 1f;
            CutoutRange = GetEvaluateValue(_fadeRate);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            OnReset();
        }
#endif
    }
}
