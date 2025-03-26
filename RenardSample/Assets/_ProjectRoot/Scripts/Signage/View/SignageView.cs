using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace SignageHADO
{
    [Serializable]
    public class SignageView : MonoBehaviourCustom
    {
        [Header("Information")]
        [SerializeField] private CanvasGroup canvasGroupInfomation = null;
        [SerializeField] private RectTransform rectInfoFrame = null;
        [SerializeField] private Image imgInfoImage = null;
        [Header("NamePlate")]
        [SerializeField] private CanvasGroup canvasGroupNamePlate = null;
        [SerializeField] private Image imgNamePlate = null;
        [SerializeField] private Image imgName = null;
        [SerializeField] private Image imgNameRole = null;
        [Header("Lines")]
        [SerializeField] private CanvasGroup canvasGroupLines = null;
        [SerializeField] private TextMeshProUGUI txtLines = null;

        private bool _visibleInfomation = false;
        public bool VisibleInfomation
        {
            get => _visibleInfomation;
            set
            {
                _visibleInfomation = value;

                if (canvasGroupInfomation != null)
                {
                    canvasGroupInfomation.alpha = _visibleInfomation ? 1f : 0f;
                    canvasGroupInfomation.blocksRaycasts = false;
                }
            }
        }

        private bool _visibleNamePlate = false;
        public bool VisibleNamePlate
        {
            get => _visibleNamePlate;
            set
            {
                _visibleNamePlate = value;

                if (canvasGroupNamePlate != null)
                {
                    canvasGroupNamePlate.alpha = _visibleNamePlate ? 1f : 0f;
                    canvasGroupNamePlate.blocksRaycasts = false;
                }
            }
        }

        private bool _visibleLinesPlate = false;
        public bool VisibleLinesPlate
        {
            get => _visibleLinesPlate;
            set
            {
                _visibleLinesPlate = value;

                if (canvasGroupLines != null)
                {
                    canvasGroupLines.alpha = _visibleLinesPlate ? 1f : 0f;
                    canvasGroupLines.blocksRaycasts = false;
                }
            }
        }

        public static float ChangeLinesTime => 0.5f;

        private Sequence _onAnimationInfoSequence = null;
        private Sequence _onAnimationNameSequence = null;
        private Sequence _onAnimationLinesSequence = null;

        private void Awake()
        {
            ResetUI();
        }

        public void ResetUI()
        {
            OnKillAnimationInfoFrame();
            OnCompletedInfoHide();

            OnKillAnimationNameFrame();
            OnCompletedNameHide();

            OnKillAnimationLinesFrame();
            OnCompletedLinesHide();
        }

        #region InformationUI

        private void OnCompletedInfoHide()
        {
            VisibleInfomation = false;
            rectInfoFrame.localScale = Vector3.zero;
            imgInfoImage.color = new Color(imgInfoImage.color.r, imgInfoImage.color.g, imgInfoImage.color.g, 0f);
        }

        private void OnCompletedInfoShow()
        {
            rectInfoFrame.localScale = Vector3.one;
            imgInfoImage.color = new Color(imgInfoImage.color.r, imgInfoImage.color.g, imgInfoImage.color.g, 1f);
            VisibleInfomation = true;
        }

        private void OnKillAnimationInfoFrame()
        {
            _onAnimationInfoSequence?.Kill();
            _onAnimationInfoSequence = null;
        }

        public void ShowInfomation(Sprite sprite, float fadeTime = 0f)
        {
            if (sprite == null)
                return;

            if (VisibleInfomation)
            {
                OnChangeInfoAnime(sprite, fadeTime);
            }
            else
            {
                imgInfoImage.sprite = sprite;
                OnShowInfoAnime(fadeTime);
            }
        }

        protected void OnShowInfoAnime(float fadeTime)
        {
            OnKillAnimationInfoFrame();

            if (fadeTime <= 0)
            {
                OnCompletedInfoShow();
                return;
            }

            _onAnimationInfoSequence = DOTween.Sequence();

            _onAnimationInfoSequence
                .OnStart(() => VisibleInfomation = true)
                .Prepend(rectInfoFrame.DOScale(Vector3.one, fadeTime * 0.7f).SetEase(Ease.InOutSine))
                .Append(imgInfoImage.DOFade(1f, fadeTime * 0.3f).SetEase(Ease.InOutSine))
                .OnComplete(() => OnCompletedInfoShow());

            _onAnimationInfoSequence.Play();
        }

        protected void OnChangeInfoAnime(Sprite sprite, float fadeTime)
        {
            OnKillAnimationInfoFrame();

            if (imgInfoImage.sprite == sprite)
                return;

            if (fadeTime <= 0)
            {
                OnCompletedInfoShow();
                imgInfoImage.sprite = sprite;
                return;
            }

            _onAnimationInfoSequence = DOTween.Sequence();

            _onAnimationInfoSequence
                .OnStart(() => OnCompletedInfoShow())
                .Prepend(imgInfoImage.DOFade(0f, fadeTime * 0.3f).SetEase(Ease.InOutSine))
                .AppendCallback(() =>
                {
                    if (imgInfoImage != null)
                        imgInfoImage.sprite = sprite;
                })
                .Append(imgInfoImage.DOFade(1f, fadeTime * 0.7f).SetEase(Ease.InOutSine))
                .OnComplete(() => OnCompletedInfoShow());

            _onAnimationInfoSequence.Play();
        }

        public void HideInfomation(float fadeTime = 0f)
        {
            if (!VisibleInfomation)
                return;

            OnHideInfoAnime(fadeTime);
        }

        protected void OnHideInfoAnime(float fadeTime)
        {
            OnKillAnimationInfoFrame();

            if (fadeTime <= 0)
            {
                OnCompletedInfoHide();
                return;
            }

            _onAnimationInfoSequence = DOTween.Sequence();

            _onAnimationInfoSequence
                .Prepend(imgInfoImage.DOFade(0f, fadeTime * 0.3f).SetEase(Ease.InOutSine))
                .Append(rectInfoFrame.DOScale(Vector3.zero, fadeTime * 0.7f).SetEase(Ease.InOutSine))
                .OnComplete(() => OnCompletedInfoHide());

            _onAnimationInfoSequence.Play();
        }

        #endregion

        #region NamePlateUI

        private void OnCompletedNameHide()
        {
            VisibleNamePlate = false;
            imgNamePlate.rectTransform.localScale = Vector3.zero;
            imgNamePlate.color = new Color(imgNamePlate.color.r, imgNamePlate.color.g, imgNamePlate.color.g, 0f);
            imgName.color = new Color(imgName.color.r, imgName.color.g, imgName.color.g, 0f);
            imgNameRole.color = new Color(imgNameRole.color.r, imgNameRole.color.g, imgNameRole.color.g, 0f);
        }

        private void OnCompletedNameShow()
        {
            imgNamePlate.rectTransform.localScale = Vector3.one;
            imgNamePlate.color = new Color(imgNamePlate.color.r, imgNamePlate.color.g, imgNamePlate.color.g, 1f);
            imgName.color = new Color(imgName.color.r, imgName.color.g, imgName.color.g, 1f);
            imgNameRole.color = new Color(imgNameRole.color.r, imgNameRole.color.g, imgNameRole.color.g, 1f);
            VisibleNamePlate = true;
        }

        private void OnKillAnimationNameFrame()
        {
            _onAnimationNameSequence?.Kill();
            _onAnimationNameSequence = null;
        }

        public void ShowNamePlate(Sprite plate, Sprite name, Sprite nameRole, float animeTime = 0f)
        {
            if (VisibleNamePlate)
                return;

            OnKillAnimationNameFrame();

            imgNamePlate.sprite = plate;
            imgName.sprite = name;
            imgNameRole.sprite = nameRole;

            if (animeTime <= 0)
            {
                OnCompletedNameShow();
                return;
            }

            _onAnimationNameSequence = DOTween.Sequence();

            _onAnimationNameSequence
                .OnStart(() =>
                {
                    OnCompletedNameHide();

                    imgNamePlate.rectTransform.localScale = new Vector3(0f, 1f, 1f);
                    VisibleNamePlate = true;
                })
                .Prepend(imgNamePlate.rectTransform.DOScaleX(1f, animeTime * 0.5f).SetEase(Ease.InOutSine))
                .Join(imgNamePlate.DOFade(1f, animeTime * 0.3f).SetEase(Ease.InOutSine))
                .Append(imgName.DOFade(1f, animeTime * 0.5f).SetEase(Ease.InOutSine))
                .Join(imgNameRole.DOFade(1f, animeTime * 0.5f).SetEase(Ease.InOutSine))
                .OnComplete(() => OnCompletedNameShow());

            _onAnimationNameSequence.Play();
        }

        public void HideNamePlate(float animeTime = 0f)
        {
            if (!VisibleNamePlate)
                return;

            OnKillAnimationNameFrame();

            if (animeTime <= 0)
            {
                OnCompletedNameHide();
                return;
            }

            _onAnimationNameSequence = DOTween.Sequence();

            _onAnimationNameSequence
                .Prepend(imgName.DOFade(0f, animeTime * 0.3f).SetEase(Ease.InOutSine))
                .Join(imgNameRole.DOFade(0f, animeTime * 0.3f).SetEase(Ease.InOutSine))
                .Append(imgNamePlate.rectTransform.DOScaleX(0f, animeTime * 0.7f).SetEase(Ease.InOutSine))
                .Join(imgNamePlate.DOFade(0f, animeTime * 0.5f).SetEase(Ease.InOutSine))
                .OnComplete(() => OnCompletedNameHide());

            _onAnimationNameSequence.Play();
        }

        #endregion

        #region Lines

        private void OnCompletedLinesHide()
        {
            txtLines.color = new Color(txtLines.color.r, txtLines.color.g, txtLines.color.b, 0f);
            txtLines.text = string.Empty;
            VisibleLinesPlate = false;
        }

        private void OnCompletedLinesShow(string lines)
        {
            txtLines.color = new Color(txtLines.color.r, txtLines.color.g, txtLines.color.b, 1f);
            txtLines.text = lines;
            VisibleLinesPlate = true;
        }

        private void OnKillAnimationLinesFrame()
        {
            _onAnimationLinesSequence?.Kill();
            _onAnimationLinesSequence = null;
        }

        public void ShowLines(string lines)
        {
            if (VisibleLinesPlate)
            {
                if (txtLines.text == lines)
                    return;
            }

            OnKillAnimationLinesFrame();

            if (string.IsNullOrEmpty(lines))
            {
                HideLines();
                return;
            }

            if (ChangeLinesTime <= 0)
            {
                OnCompletedLinesShow(lines);
                return;
            }

            _onAnimationLinesSequence = DOTween.Sequence();

            if (VisibleLinesPlate)
            {
                _onAnimationLinesSequence
                    .Prepend(txtLines.DOFade(0f, ChangeLinesTime * 0.5f).SetEase(Ease.InOutSine))
                    .AppendCallback(() =>
                    {
                        txtLines.text = lines;
                    })
                    .Append(txtLines.DOFade(1f, ChangeLinesTime * 0.5f).SetEase(Ease.InOutSine))
                    .OnComplete(() => OnCompletedLinesShow(lines));
            }
            else
            {
                _onAnimationLinesSequence
                    .OnStart(() =>
                    {
                        VisibleLinesPlate = true;
                        txtLines.color = new Color(txtLines.color.r, txtLines.color.g, txtLines.color.b, 0f);
                        txtLines.text = lines;
                    })
                    .Prepend(txtLines.DOFade(1f, ChangeLinesTime).SetEase(Ease.InOutSine))
                    .OnComplete(() => OnCompletedLinesShow(lines));
            }

            _onAnimationLinesSequence.Play();
        }

        public void HideLines()
        {
            if (!VisibleLinesPlate)
                return;

            OnKillAnimationLinesFrame();
            OnCompletedLinesHide();
        }

        #endregion
    }
}
