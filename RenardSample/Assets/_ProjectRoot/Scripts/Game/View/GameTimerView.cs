using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using TMPro;

namespace SignageHADO
{
    [Serializable]
    public class GameTimerView : MonoBehaviourCustom
    {
        [SerializeField] private CanvasGroup canvasGroupFrame = default;
        [SerializeField] private RectTransform frameBaseRect = default;
        [SerializeField] private CanvasGroup canvasGroupTimerValue = default;
        [SerializeField] private Image timerGage = default;
        [SerializeField] private Image minutes10 = default;
        [SerializeField] private Image minutes1 = default;
        [SerializeField] private Image seconds10 = default;
        [SerializeField] private Image seconds1 = default;
        [SerializeField] private Sprite[] timerSprites = default;
        [SerializeField] private float animaShowTime = 1f;
        [SerializeField] private float animaHideTime = 0.5f;

        private float _visibleAlpha = 0f;
        protected float visibleAlpha
        {
            get => _visibleAlpha;
            set
            {
                _visibleAlpha = value;

                if (canvasGroupFrame != null)
                {
                    canvasGroupFrame.alpha = _visibleAlpha;
                    canvasGroupFrame.blocksRaycasts = false;
                }
            }
        }

        private float _visibleTimerValue = 0f;
        protected float visibleTimerValue
        {
            get => _visibleTimerValue;
            set
            {
                _visibleTimerValue = value;

                if (canvasGroupTimerValue != null)
                {
                    canvasGroupTimerValue.alpha = _visibleTimerValue;
                    canvasGroupTimerValue.blocksRaycasts = false;
                }
            }
        }

        private GameStatus gameStatus => GameHandler.GameStatus;
        private float gameTime => GameHandler.GameTime;
        private float fillAmountGameTime => GameHandler.FillAmountGameTime;

        [HideInInspector] private bool _isShow = false;
        [HideInInspector] private int _timerMinutes = 0;
        [HideInInspector] private int _timerSeconds = 0;
        [HideInInspector] private int _minutes10 = 0;
        [HideInInspector] private int _minutes1 = 0;
        [HideInInspector] private int _seconds10 = 0;
        [HideInInspector] private int _seconds1 = 0;

        private Sequence _onAnimationFrameSequence = null;

        private void Start()
        {
            ResetAnimationFrame();

            this.ObserveEveryValueChanged(x => x.gameStatus)
                .Subscribe(ChangeGameStatus)
                .AddTo(this);

            ChangeGameStatus(gameStatus);
        }

        private void ChangeGameStatus(GameStatus status)
        {
            if (status == GameStatus.Start || status == GameStatus.Playing)
                OnShow(animaShowTime);

            if (status == GameStatus.End || status == GameStatus.Result)
                OnHide(animaHideTime);
        }

        private void Update()
        {
            if (visibleAlpha <= 0)
                return;

            if (gameStatus == GameStatus.None || gameStatus == GameStatus.Entry)
            {
                ResetAnimationFrame();
                return;
            }

            UpdateTimerSprite(gameTime * fillAmountGameTime);

            if (timerGage != null)
            {
                if (timerGage.fillAmount != fillAmountGameTime)
                    timerGage.fillAmount = fillAmountGameTime;
            }
        }

        private void UpdateTimerSprite(float time)
        {
            _timerMinutes = Mathf.FloorToInt(time / 60f);
            _timerSeconds = Mathf.CeilToInt(time % 60f);

            _minutes1 = _timerMinutes % 10;
            _timerMinutes /= 10;
            _minutes10 = _timerMinutes % 10;

            _seconds1 = _timerSeconds % 10;
            _timerSeconds /= 10;
            _seconds10 = _timerSeconds % 10;

            if (minutes10 != null && minutes10.sprite != timerSprites[_minutes10])
                minutes10.sprite = timerSprites[_minutes10];

            if (minutes1 != null && minutes1.sprite != timerSprites[_minutes1])
                minutes1.sprite = timerSprites[_minutes1];

            if (seconds10 != null && seconds10.sprite != timerSprites[_seconds10])
                seconds10.sprite = timerSprites[_seconds10];

            if (seconds1 != null && seconds1.sprite != timerSprites[_seconds1])
                seconds1.sprite = timerSprites[_seconds1];
        }

        private void ResetAnimationFrame()
        {
            OnKillAnimationFrame();

            frameBaseRect.localScale = Vector3.zero;
            _isShow = false;
            visibleAlpha = 0f;
            visibleTimerValue = 0f;
        }

        private void OnKillAnimationFrame()
        {
            _onAnimationFrameSequence?.Kill();
            _onAnimationFrameSequence = null;
        }

        protected void OnShow(float animaTime)
        {
            if (_isShow)
                return;

            _isShow = true;

            OnKillAnimationFrame();

            if (animaTime <= 0)
            {
                frameBaseRect.localScale = Vector3.one;
                visibleAlpha = 1f;
                visibleTimerValue = 1f;
                return;
            }

            _onAnimationFrameSequence = DOTween.Sequence();

            _onAnimationFrameSequence
                .OnStart(() =>
                {
                    frameBaseRect.localScale = Vector3.zero;
                    visibleAlpha = 0f;
                    visibleTimerValue = 0f;
                })
                .Prepend(frameBaseRect.DOScale(Vector3.one, animaTime * 0.2f).SetEase(Ease.InOutSine))
                .Join(DOTween.To(() => visibleAlpha, n => visibleAlpha = n, 1f, animaTime * 0.2f).SetEase(Ease.InOutSine))
                .Append(DOTween.To(() => visibleTimerValue, n => visibleTimerValue = n, 1f, animaTime * 0.8f).SetEase(Ease.InOutSine));

            _onAnimationFrameSequence.Play();
        }

        protected void OnHide(float animaTime)
        {
            if (!_isShow)
                return;

            _isShow = false;

            OnKillAnimationFrame();

            if (animaTime <= 0)
            {
                frameBaseRect.localScale = Vector3.zero;
                visibleAlpha = 0f;
                visibleTimerValue = 0f;
                return;
            }

            _onAnimationFrameSequence = DOTween.Sequence();

            _onAnimationFrameSequence
                .Prepend(DOTween.To(() => visibleTimerValue, n => visibleTimerValue = n, 0f, animaTime * 0.8f).SetEase(Ease.InOutSine))
                .Append(frameBaseRect.DOScale(Vector3.zero, animaTime * 0.2f).SetEase(Ease.InOutSine))
                .Join(DOTween.To(() => visibleAlpha, n => visibleAlpha = n, 0f, animaTime * 0.2f).SetEase(Ease.InOutSine))
                .OnComplete(() =>
                {
                    ResetAnimationFrame();
                });

            _onAnimationFrameSequence.Play();
        }
    }
}
