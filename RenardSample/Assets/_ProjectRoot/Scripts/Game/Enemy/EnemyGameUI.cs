using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace SignageHADO.Game
{
    [Serializable]
    public class EnemyGameUI : MonoBehaviourCustom
    {
        [SerializeField] private Canvas canvas = null;
        [Header("PointUI")]
        [SerializeField] private CanvasGroup canvasGroupPointUI = null;
        [SerializeField] private RectTransform pointValue = null;
        [Header("桁の大きい順")]
        [SerializeField] private Image[] points = null;
        [SerializeField] private Image pointUnit = null;
        [Header("0～9の順")]
        [SerializeField] private Sprite[] pointSprites = null;

        private bool _visiblePointUI = false;
        protected bool visiblePointUI
        {
            get => _visiblePointUI;
            set
            {
                _visiblePointUI = value;

                if(canvasGroupPointUI != null)
                {
                    canvasGroupPointUI.alpha = _visiblePointUI ? 1f: 0f;
                    canvasGroupPointUI.blocksRaycasts = false;
                }
            }
        }

        protected Camera worldCamera { get; private set; } = null;

        private Sequence _onPointAnimeSequence = null;

        private void Awake()
        {
            HidePoint();
        }

        public void Setup(Camera targetCamera, int point)
        {
            worldCamera = targetCamera;
            canvas.worldCamera = worldCamera;

            SetPoint(point);
        }

        public void SetPoint(int point)
        {
            var number = point;
            var pointIndex = new int[points.Length];

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
                points[i].sprite = pointSprites[pointIndex[i]];
                points[i].gameObject.SetActive((headIndex <= i));
            }
        }

        private void Update()
        {
            if (!visiblePointUI)
                return;

            transform.LookAt(worldCamera.transform.position);
        }

        private void KillAnimePoint()
        {
            _onPointAnimeSequence?.Kill();
            _onPointAnimeSequence = null;
        }

        private void ResetPoint()
        {
            visiblePointUI = false;

            pointValue.localScale = Vector3.zero;

            foreach (var item in points)
            {
                item.color = new Color(item.color.r, item.color.g, item.color.b, 0f);
            }

            pointUnit.color = new Color(pointUnit.color.r, pointUnit.color.g, pointUnit.color.b, 0f);
        }

        private void HidePoint()
        {
            KillAnimePoint();
            ResetPoint();
        }

        public void DrawPoint(float animeTime)
        {
            if (visiblePointUI)
                return;

            visiblePointUI = true;
            OnCreatePlayAnimePoint(animeTime);
        }

        private void OnCreatePlayAnimePoint(float animeTime)
        {
            KillAnimePoint();

            _onPointAnimeSequence = DOTween.Sequence();

            // アニメーションを作る

            _onPointAnimeSequence
                .Prepend(pointValue.DOScale(Vector3.one, animeTime * 0.3f).SetEase(Ease.InOutSine))
                .Join(pointUnit.DOFade(1f, animeTime * 0.1f).SetEase(Ease.InOutSine));

            foreach (var item in points)
            {
                if (!item.gameObject.activeSelf)
                    continue;

                _onPointAnimeSequence.Join(item.DOFade(1f, animeTime * 0.2f).SetEase(Ease.InOutSine));
            }

            _onPointAnimeSequence
                .AppendInterval(animeTime * 0.6f)
                .Append(pointUnit.DOFade(0f, animeTime * 0.1f).SetEase(Ease.InOutSine));

            foreach (var item in points)
            {
                if (!item.gameObject.activeSelf)
                    continue;

                _onPointAnimeSequence.Join(item.DOFade(0f, animeTime * 0.1f).SetEase(Ease.InOutSine));
            }

            _onPointAnimeSequence
                .OnComplete(() =>
                {
                    ResetPoint();
                });

            _onPointAnimeSequence.Play();
        }
    }
}
