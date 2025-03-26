using System;
using UnityEngine;
using UnityEngine.UI;

namespace SignageHADO.Game
{
    public class ReticleView : MonoBehaviourCustom
    {
        [SerializeField] private CanvasGroup canvasGroup = null;
        [SerializeField] private Image imgReticle = null;
        [SerializeField] private Sprite imgPaper = null;
        [SerializeField] private Sprite imgRock = null;
        [SerializeField] private Color colorReticleNormal = new Color32(0xFF, 0xFF, 0xFF, 0xFF);
        [SerializeField] private Color colorReticleLock = new Color32(0xFF, 0x00, 0x66, 0xFF);
        [SerializeField, Range(0f, 1f)] private float slerp = 0.9f;
        [Header("ﾌｨﾙﾀｰ設定")]
        [SerializeField] private bool filter = true;
        [SerializeField, Range(0f, 1f), Tooltip(EMAFilter.EMAFilterAlphaToolTip)] private float emaFilterAlpha = 0.2f;

        protected GameStatus gameStatus => GameHandler.GameStatus;
        protected Camera viewCamera => GameHandler.ViewCamera;

        protected bool activeView
        {
            get => imgReticle != null ? imgReticle.enabled : false;
            set
            {
                if (imgReticle != null)
                    imgReticle.enabled = value;

                if (!value)
                    visibleAlpha = 0f;
            }
        }

        private float _viewAlpha = 0f;
        protected float visibleAlpha
        {
            get => _viewAlpha;
            set
            {
                _viewAlpha = value;

                if (canvasGroup != null)
                {
                    canvasGroup.alpha = _viewAlpha;
                    canvasGroup.blocksRaycasts = false;
                }
            }
        }

        private RectTransform _frameRect = null;
        private EMAFilterVector2 _emaFilter = null;
        private Vector2 _localPoint = Vector2.zero;

        private void Awake()
        {
            _frameRect = GetComponent<RectTransform>();
            _emaFilter = new EMAFilterVector2(Vector2.one * emaFilterAlpha);
            visibleAlpha = 0f;
        }

        public Vector2 UpdateReticle(Vector2 markingViewPos, HandPoseEnum handPose, bool lockEnemy, bool active)
        {
            if (activeView != active)
                activeView = active;

            if (!activeView)
                return new Vector2(Screen.width, Screen.height) * 0.5f;

            if (viewCamera != null && _frameRect != null)
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_frameRect, viewCamera.ViewportToScreenPoint(markingViewPos), viewCamera, out _localPoint);

            if (filter)
                _localPoint = _emaFilter.Apply(_localPoint);

            if (handPose == HandPoseEnum.Paper)
            {
                if (imgReticle.sprite != imgPaper)
                    imgReticle.sprite = imgPaper;
            }
            else
            {
                if (imgReticle.sprite != imgRock)
                    imgReticle.sprite = imgRock;
            }

            visibleAlpha = 1f;
            imgReticle.rectTransform.localPosition = Vector3.Slerp(imgReticle.rectTransform.localPosition, _localPoint, slerp);
            imgReticle.color = (lockEnemy ? colorReticleLock : colorReticleNormal);

            return viewCamera.WorldToViewportPoint(imgReticle.transform.position, Camera.MonoOrStereoscopicEye.Mono);
        }
    }
}
