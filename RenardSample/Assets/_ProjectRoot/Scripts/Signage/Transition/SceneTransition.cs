using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum TransitionFadeMode
{
    /// <summary>FadeIn</summary>
    FadeIn,
    /// <summary>FadeOut</summary>
    FadeOut
}

public enum TransitionType
{
    /// <summary>Cutout</summary>
    Cutout,
    /// <summary>Alpha</summary>
    Alpha,
    /// <summary>Mask</summary>
    Mask
}

[Serializable]
public class TransitionParameter
{
    public TransitionFadeMode Mode = TransitionFadeMode.FadeIn;
    public TransitionType Type = TransitionType.Mask;
    public Texture2D RuleImage = default;
    public AnimationCurve Curve = default;

    public const float DefaultFadeTime = 0.5f;
}

namespace SignageHADO
{
    [Serializable]
    public sealed class SceneTransition : MonoBehaviourCustom
    {
        [SerializeField] private SceneTransitionImage _fadeImage = default;
        [SerializeField] private TransitionParameter _fadeIn = default;
        [SerializeField] private TransitionParameter _fadeOut = default;
        [SerializeField] private Color _graphicColor = Color.black;

        public TransitionParameter FadeIn => _fadeIn;
        public TransitionParameter FadeOut => _fadeOut;

        private TransitionFadeMode currentMode => _fadeImage != null ? _fadeImage.CurrentMode : TransitionFadeMode.FadeOut;

        private TransitionParameter _nowParameter = null;
        private RectTransform _targetRectTransform = null;
        private RectTransform _parentRectTransform = null;

        [HideInInspector] private Rect _parentRect = default;
        [HideInInspector] private float _width = 0f;
        [HideInInspector] private float _height = 0f;
        [HideInInspector] private float _ratio = 0f;
        [HideInInspector] private Rect _rect = default;
        [HideInInspector] private Vector2 _center = Vector2.zero;
        [HideInInspector] private Vector3 _rotatedTopLeftRel = Vector3.zero;
        [HideInInspector] private Vector3 _rotatedTopRightRel = Vector3.zero;

        private CancellationTokenSource _cancellationTokenSource = null;

        private void Awake()
        {
            _targetRectTransform = _fadeImage.GetComponent<RectTransform>();
            _parentRectTransform = GetComponent<RectTransform>();
        }

        private void LateUpdate()
        {
            UpdateAutoFit();
        }

        private void UpdateAutoFit()
        {
            if (_targetRectTransform == null || _parentRectTransform == null)
                return;

            _rect = _targetRectTransform.rect;
            _parentRect = _parentRectTransform.rect;

            if (_rect.width == 0 || _rect.height == 0)
                return;

            _center = _rect.center;
            _rotatedTopLeftRel = _targetRectTransform.localRotation * new Vector2(_rect.xMin - _center.x, _rect.yMin - _center.y);
            _rotatedTopRightRel = _targetRectTransform.localRotation * new Vector2(_rect.xMax - _center.x, _rect.yMin - _center.y);

            _width = 2 * Mathf.Max(Mathf.Abs(_rotatedTopLeftRel.x), Mathf.Abs(_rotatedTopRightRel.x));
            _height = 2 * Mathf.Max(Mathf.Abs(_rotatedTopLeftRel.y), Mathf.Abs(_rotatedTopRightRel.y));

            if ((_parentRect.width / _width) <= (_parentRect.height / _height))
            {
                _ratio = _parentRect.height / _height;
            }
            else
            {
                _ratio = _parentRect.width / _width;
            }

            _targetRectTransform.offsetMin *= _ratio;
            _targetRectTransform.offsetMax *= _ratio;
            _targetRectTransform.localPosition = Vector3.zero;
        }

        private void OnDestroy()
        {
            Dispose();
        }

        public void Reset()
        {
            Dispose();

            _fadeImage?.Reset();
            _nowParameter = null;
        }

        private void Dispose()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Dispose();
                _nowParameter = null;
            }

            _cancellationTokenSource = null;
        }

        private async UniTask OnPlayTransition(CancellationToken token, Color graphicColor, float fadeTime, float delayTime)
        {
            if (_fadeImage != null && _nowParameter != null)
            {
                _fadeImage.Reset();

                if (_nowParameter.RuleImage != null)
                    _fadeImage.MaskTexture = _nowParameter.RuleImage;

                if (_targetRectTransform != null)
                    _targetRectTransform.sizeDelta = _fadeImage.MaskTexture.texelSize;

                await _fadeImage.PlayFade(token, graphicColor, _nowParameter, fadeTime, delayTime);
            }

            _nowParameter = null;
        }

        private void OnPlayTransitionToUniTask(Color graphicColor, TransitionParameter parameter, float fadeTime, float delayTime)
        {
            try
            {
                if (currentMode == TransitionFadeMode.FadeIn)
                {
                    if (parameter.Mode == TransitionFadeMode.FadeIn)
                        return;
                }
                else
                {
                    if (parameter.Mode == TransitionFadeMode.FadeOut)
                        return;
                }

                Reset();

                _nowParameter = parameter;

                _cancellationTokenSource = new CancellationTokenSource();
                OnPlayTransition(_cancellationTokenSource.Token, graphicColor, fadeTime, delayTime).Forget();
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Error, "OnPlayTransitionToUniTask", $"error. {ex.Message}");
                _nowParameter = null;
            }
        }

        public void PlayFadeIn(float fadeTime = TransitionParameter.DefaultFadeTime, float delayTime = 0f)
            => PlayFadeIn(_graphicColor, fadeTime, delayTime);

        public void PlayFadeIn(Color graphicColor, float fadeTime = TransitionParameter.DefaultFadeTime,  float delayTime = 0f)
            => OnPlayTransitionToUniTask(graphicColor, FadeIn, fadeTime, delayTime);

        public void PlayFadeOut(float fadeTime = TransitionParameter.DefaultFadeTime, float delayTime = 0f)
            => PlayFadeOut(_graphicColor, fadeTime, delayTime);

        public void PlayFadeOut(Color graphicColor, float fadeTime = TransitionParameter.DefaultFadeTime, float delayTime = 0f)
            => OnPlayTransitionToUniTask(graphicColor, FadeOut, fadeTime, delayTime);

        public void Play(TransitionParameter parameter, float fadeTime = TransitionParameter.DefaultFadeTime, float delayTime = 0f)
            => Play(_graphicColor, parameter, fadeTime, delayTime);

        public void Play(Color graphicColor, TransitionParameter parameter, float fadeTime = TransitionParameter.DefaultFadeTime, float delayTime = 0f)
            => OnPlayTransitionToUniTask(graphicColor, parameter, fadeTime, delayTime);
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(SceneTransition))]
    public class SceneTransitionEditor : Editor
    {
        private SceneTransition transitionUI => target as SceneTransition;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!EditorApplication.isPlaying)
                return;

            if (GUILayout.Button("PlayFadeIn"))
            {
                transitionUI?.PlayFadeIn();
            }

            if (GUILayout.Button("PlayFadeOut"))
            {
                transitionUI?.PlayFadeOut();
            }
        }
    }

#endif
}
