using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SignageHADO.UI
{
    [Serializable]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(RectTransform))]
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class UI3DCanvas : MonoBehaviourCustom
    {
        private Canvas _canvas = default;

        protected Canvas canvas => _canvas ?? (_canvas = GetComponent<Canvas>());

        private RectTransform _rectTransform = default;

        protected RectTransform rectTransform => _rectTransform ?? (_rectTransform = GetComponent<RectTransform>());

        private UI3DFrame _ui3DFrame = default;
        protected UI3DFrame ui3DFrame => _ui3DFrame ?? (_ui3DFrame = SearchUI3DFrame(transform));

        private void OnEnable()
        {
            OnUpdateScale();
        }

        private void Update()
        {
            OnUpdateScale();
        }

        private void OnUpdateScale()
        {
            transform.localScale = ui3DFrame != null ? ui3DFrame.ResizeScele : Vector3.one;
        }

        private UI3DFrame SearchUI3DFrame(Transform current)
        {
            var frame = current.GetComponent<UI3DFrame>();
            if (frame != null)
                return frame;

            if (current.parent != null)
                return SearchUI3DFrame(current.parent);

            return null;
        }
    }
}
