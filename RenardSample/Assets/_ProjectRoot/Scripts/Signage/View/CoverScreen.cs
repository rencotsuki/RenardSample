using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[Serializable]
public class CoverScreen : MonoBehaviourCustom
{
    [SerializeField] private CanvasGroup canvasGroup = null;
    [SerializeField] private Image imgBackground = null;
    [SerializeField] private Color defaultColor = Color.black;
    [SerializeField] private Image imgIcon = null;

    public bool Visible { get; private set; } = false;

    private float _alphaValue = 0f;
    private Tweener _onFadeTween = null;

    private void Awake()
    {
        SetVisible(Visible);
    }

    private void KillFadeTween()
    {
        _onFadeTween?.Kill();
        _onFadeTween = null;
    }

    private void SetVisible(bool visible)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.blocksRaycasts = visible;
        }
    }

    public void Show(float fade = 0f) => Show(fade, defaultColor);

    public void Show(float fade, Color background)
    {
        if (Visible)
            return;

        Visible = true;

        imgBackground.color = background;

        KillFadeTween();

        if (fade <= 0)
        {
            SetVisible(Visible);
        }
        else
        {
            _onFadeTween = DOTween.To(() => _alphaValue, x => _alphaValue = x, 1f, fade)
                .SetEase(Ease.InOutSine)
                .OnUpdate(() =>
                {
                    if (canvasGroup != null)
                        canvasGroup.alpha = _alphaValue;
                })
                .OnComplete(() =>
                {
                    SetVisible(Visible);
                });

            _onFadeTween.Play();
        }
    }

    public void Hide(float fade = 0f)
    {
        if (!Visible)
            return;

        Visible = false;

        KillFadeTween();

        if (fade <= 0)
        {
            SetVisible(Visible);
        }
        else
        {
            _onFadeTween = DOTween.To(() => _alphaValue, x => _alphaValue = x, 0f, fade)
                .SetEase(Ease.InOutSine)
                .OnUpdate(() =>
                {
                    if (canvasGroup != null)
                        canvasGroup.alpha = _alphaValue;
                })
                .OnComplete(() =>
                {
                    SetVisible(Visible);
                });

            _onFadeTween.Play();
        }
    }

    public void SetRoll(float roll)
    {
        if (imgIcon != null && imgIcon.rectTransform.localEulerAngles.z != -roll)
            imgIcon.rectTransform.localEulerAngles = new Vector3(imgIcon.rectTransform.localEulerAngles.x, imgIcon.rectTransform.localEulerAngles.y, -roll);
    }
}