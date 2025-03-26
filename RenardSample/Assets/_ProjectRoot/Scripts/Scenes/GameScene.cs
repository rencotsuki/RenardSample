using System;
using UnityEngine;

namespace SignageHADO
{
    using UI;

    [Serializable]
    public class GameScene : MonoBehaviourCustom
    {
        [Header("-- UI3D")]
        [SerializeField] private Canvas uiCanvas = null;
        [SerializeField] private UI3DCanvasScaler ui3DCanvasScaler = null;
        [SerializeField] private UI3DFrame ui3DFrame = null;

        protected const int loadWaitTimeMilliseconds = 3 * 1000;

        protected Camera displayCamera => SystemHandler.DisplayCamera;

        private float displaySizeW => ConfigSignageHADO.DisplaySizeW;
        private float displaySizeH => ConfigSignageHADO.DisplaySizeH;

        private Vector2 _tmpDisplaySize = new Vector2();

        private void Start()
        {
            uiCanvas.worldCamera = displayCamera;
            ui3DFrame.SetCamera(displayCamera);

            _tmpDisplaySize = new Vector2(displaySizeW, displaySizeH);
        }

        private void Update()
        {
            if (_tmpDisplaySize.x != displaySizeW)
                _tmpDisplaySize.x = displaySizeW;

            if (_tmpDisplaySize.y != displaySizeH)
                _tmpDisplaySize.y = displaySizeH;

            if (ui3DCanvasScaler != null && ui3DCanvasScaler.ResolutionSize != _tmpDisplaySize)
                ui3DCanvasScaler.Set(_tmpDisplaySize, ui3DCanvasScaler.PixelsPerUnit, ui3DCanvasScaler.MatchMode);

            if (ui3DFrame != null && ui3DFrame.TargetPixel != _tmpDisplaySize)
                ui3DFrame.TargetPixel = _tmpDisplaySize;
        }
    }
}
