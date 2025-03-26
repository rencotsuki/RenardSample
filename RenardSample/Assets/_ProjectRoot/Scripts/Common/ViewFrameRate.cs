using UnityEngine;
using UniRx;
using TMPro;

namespace SignageHADO
{
    public class ViewFrameRate : MonoBehaviourCustom
    {
        [SerializeField] private TMP_Text txtView = null;

        private float frameRate => SystemHandler.FrameRate;

        private void Start()
        {
            this.ObserveEveryValueChanged(x => x.frameRate)
                .ThrottleFrame(1)
                .Subscribe(UpdateView)
                .AddTo(this);
        }

        private void UpdateView(float rate)
        {
            if (txtView != null)
                txtView.text = $"fps:{(rate <= 0 ? "---" : $"{rate:#0.0}")}";
        }
    }
}
