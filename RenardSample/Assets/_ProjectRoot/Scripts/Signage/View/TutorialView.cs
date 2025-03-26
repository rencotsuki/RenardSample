using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace SignageHADO
{
    [Serializable]
    public class TutorialView : MonoBehaviourCustom
    {
        [SerializeField] private CanvasGroup canvasGroup = null;
        [SerializeField] private RectTransform rectFrame = null;
        [SerializeField] private Image imgTutorial = null;

        private bool _visible = false;
        public bool Visible
        {
            get => _visible;
            set
            {
                _visible = value;

                if (canvasGroup != null)
                {
                    canvasGroup.alpha = _visible ? 1f : 0f;
                    canvasGroup.blocksRaycasts = false;
                }
            }
        }

        private Sequence _onAnimationInfoSequence = null;

        private void Awake()
        {
            ResetUI();
        }

        public void ResetUI()
        {
            OnKillAnimationFrame();
            OnCompletedHide();
        }

        #region InformationUI

        private void OnCompletedHide()
        {
            Visible = false;
            rectFrame.localScale = Vector3.zero;
            imgTutorial.color = new Color(imgTutorial.color.r, imgTutorial.color.g, imgTutorial.color.g, 0f);
        }

        private void OnCompletedShow()
        {
            rectFrame.localScale = Vector3.one;
            imgTutorial.color = new Color(imgTutorial.color.r, imgTutorial.color.g, imgTutorial.color.g, 1f);
            Visible = true;
        }

        private void OnKillAnimationFrame()
        {
            _onAnimationInfoSequence?.Kill();
            _onAnimationInfoSequence = null;
        }

        public void ShowTutorial(Sprite sprite, float fadeTime = 0f)
        {
            if (sprite == null)
                return;

            if (Visible)
            {
                OnChangeAnime(sprite, fadeTime);
            }
            else
            {
                imgTutorial.sprite = sprite;
                OnShowAnime(fadeTime);
            }
        }

        protected void OnShowAnime(float fadeTime)
        {
            OnKillAnimationFrame();

            if (fadeTime <= 0)
            {
                OnCompletedShow();
                return;
            }

            _onAnimationInfoSequence = DOTween.Sequence();

            _onAnimationInfoSequence
                .OnStart(() => Visible = true)
                .Prepend(rectFrame.DOScale(Vector3.one, fadeTime * 0.7f).SetEase(Ease.InOutSine))
                .Append(imgTutorial.DOFade(1f, fadeTime * 0.3f).SetEase(Ease.InOutSine))
                .OnComplete(() => OnCompletedShow());

            _onAnimationInfoSequence.Play();
        }

        protected void OnChangeAnime(Sprite sprite, float fadeTime)
        {
            OnKillAnimationFrame();

            if (imgTutorial.sprite == sprite)
                return;

            if (fadeTime <= 0)
            {
                OnCompletedShow();
                imgTutorial.sprite = sprite;
                return;
            }

            _onAnimationInfoSequence = DOTween.Sequence();

            _onAnimationInfoSequence
                .OnStart(() => OnCompletedShow())
                .Prepend(imgTutorial.DOFade(0f, fadeTime * 0.3f).SetEase(Ease.InOutSine))
                .AppendCallback(() =>
                {
                    if (imgTutorial != null)
                        imgTutorial.sprite = sprite;
                })
                .Append(imgTutorial.DOFade(1f, fadeTime * 0.7f).SetEase(Ease.InOutSine))
                .OnComplete(() => OnCompletedShow());

            _onAnimationInfoSequence.Play();
        }

        public void HideTutorial(float fadeTime = 0f)
        {
            if (!Visible)
                return;

            OnHideAnime(fadeTime);
        }

        protected void OnHideAnime(float fadeTime)
        {
            OnKillAnimationFrame();

            if (fadeTime <= 0)
            {
                OnCompletedHide();
                return;
            }

            _onAnimationInfoSequence = DOTween.Sequence();

            _onAnimationInfoSequence
                .Prepend(imgTutorial.DOFade(0f, fadeTime * 0.3f).SetEase(Ease.InOutSine))
                .Append(rectFrame.DOScale(Vector3.zero, fadeTime * 0.7f).SetEase(Ease.InOutSine))
                .OnComplete(() => OnCompletedHide());

            _onAnimationInfoSequence.Play();
        }

        #endregion
    }
}
