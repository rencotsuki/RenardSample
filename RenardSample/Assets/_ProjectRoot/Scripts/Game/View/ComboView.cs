using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;

namespace SignageHADO
{
    [Serializable]
    public class ComboView : MonoBehaviourCustom
    {
        [SerializeField] private CanvasGroup canvasGroup = default;
        [SerializeField] private CanvasGroup canvasGroupComboValue = default;
        [SerializeField] private RectTransform countValue = default;
        [Header("桁の大きい順")]
        [SerializeField] private Image[] counts = default;
        [Header("0～9の順")]
        [SerializeField] private Sprite[] comboSprites = default;
        [SerializeField] private float animaShowTime = 0.5f;
        [SerializeField] private float animaHideTime = 0.3f;
        [SerializeField] private float animaChangeTime = 0.3f;

        private float _visibleAlpha = 0f;
        protected float visibleAlpha
        {
            get => _visibleAlpha;
            set
            {
                _visibleAlpha = value;

                if (canvasGroup != null)
                {
                    canvasGroup.alpha = _visibleAlpha;
                    canvasGroup.blocksRaycasts = false;
                }
            }
        }

        private float _visibleComboValue = 0f;
        protected float visibleComboValue
        {
            get => _visibleComboValue;
            set
            {
                _visibleComboValue = value;

                if (canvasGroupComboValue != null)
                {
                    canvasGroupComboValue.alpha = _visibleComboValue;
                    canvasGroupComboValue.blocksRaycasts = false;
                }
            }
        }

        private GameStatus gameStatus => GameHandler.GameStatus;
        private int comboCount => GameHandler.ComboCount;

        [HideInInspector] private bool _isShow = false;

        private Sequence _onAnimationComboSequence = null;

        private void Start()
        {
            ResetComboView();

            this.ObserveEveryValueChanged(x => x.comboCount)
                .Subscribe(ChangeComboCount)
                .AddTo(this);

            this.ObserveEveryValueChanged(x => x.gameStatus)
                .Subscribe(ChangeGameStatus)
                .AddTo(this);
        }

        private void ChangeGameStatus(GameStatus status)
        {
            if (status != GameStatus.Playing)
                OnHide(animaHideTime);
        }

        private void ChangeComboCount(int count)
        {
            if (gameStatus != GameStatus.Playing)
                return;

            if (count <= 0)
            {
                OnHide(animaHideTime);
            }
            else
            {
                if (_isShow)
                {
                    OnChangeDraw(animaChangeTime, count);
                }
                else
                {
                    OnShow(animaShowTime, count);
                }
            }
        }

        public void SetCount(int count)
        {
            var number = count;
            var pointIndex = new int[counts.Length];

            var headIndex = pointIndex.Length - 1;
            for (int i = pointIndex.Length - 1; i >= 0; i--)
            {
                pointIndex[i] = number % 10;
                number /= 10;

                if (pointIndex[i] > 0 && headIndex > i)
                    headIndex = i;
            }

            for (int i = 0; i < pointIndex.Length; i++)
            {
                counts[i].sprite = comboSprites[pointIndex[i]];
                counts[i].gameObject.SetActive((headIndex <= i));
            }
        }

        private void ResetComboView()
        {
            OnKillAnimationCombo();

            _isShow = false;
            visibleAlpha = 0f;
            visibleComboValue = 0f;
        }

        private void OnKillAnimationCombo()
        {
            _onAnimationComboSequence?.Kill();
            _onAnimationComboSequence = null;
        }

        protected void OnShow(float animaTime, int count)
        {
            _isShow = true;

            OnKillAnimationCombo();

            SetCount(count);

            if (animaTime <= 0)
            {
                visibleAlpha = 1f;
                visibleComboValue = 1f;
                return;
            }

            _onAnimationComboSequence = DOTween.Sequence();

            _onAnimationComboSequence
                .OnStart(() =>
                {
                    countValue.localScale = Vector3.one * 1.5f;
                    visibleAlpha = 0f;
                    visibleComboValue = 0f;
                })
                .Prepend(DOTween.To(() => visibleAlpha, n => visibleAlpha = n, 1f, animaTime * 0.2f).SetEase(Ease.InCirc))
                .Append(countValue.DOScale(Vector3.one, animaTime * 0.8f).SetEase(Ease.InCirc))
                .Join(DOTween.To(() => visibleComboValue, n => visibleComboValue = n, 1f, animaTime * 0.8f).SetEase(Ease.InOutSine));

            _onAnimationComboSequence.Play();
        }

        protected void OnHide(float animaTime)
        {
            if (!_isShow)
                return;

            _isShow = false;

            OnKillAnimationCombo();

            if (animaTime <= 0)
            {
                visibleAlpha = 0f;
                visibleComboValue = 0f;
                return;
            }

            _onAnimationComboSequence = DOTween.Sequence();

            _onAnimationComboSequence
                .Prepend(DOTween.To(() => visibleAlpha, n => visibleAlpha = n, 0f, animaTime).SetEase(Ease.InOutSine))
                .OnComplete(() =>
                {
                    ResetComboView();
                });

            _onAnimationComboSequence.Play();
        }

        protected void OnChangeDraw(float animaTime, int count)
        {
            if (!_isShow)
                return;

            OnKillAnimationCombo();
            visibleAlpha = 1f;

            if (animaTime <= 0)
            {
                SetCount(count);
                return;
            }

            _onAnimationComboSequence = DOTween.Sequence();

            _onAnimationComboSequence
                .Prepend(DOTween.To(() => visibleComboValue, n => visibleComboValue = n, 0f, animaTime * 0.2f).SetEase(Ease.InCirc))
                .AppendCallback(() =>
                {
                    countValue.localScale = Vector3.one * 1.5f;
                    SetCount(count);
                })
                .Append(countValue.DOScale(Vector3.one, animaTime * 0.8f).SetEase(Ease.InCirc))
                .Join(DOTween.To(() => visibleComboValue, n => visibleComboValue = n, 1f, animaTime * 0.8f).SetEase(Ease.InOutSine));

            _onAnimationComboSequence.Play();
        }

    }
}
