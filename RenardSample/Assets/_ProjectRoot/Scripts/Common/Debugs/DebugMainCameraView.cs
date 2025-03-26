using System;
using UnityEngine;

namespace SignageHADO
{
    public class DebugMainCameraView : MonoBehaviourCustom
    {
        protected RenderTexture outputTexture = null;
        public RenderTexture OutputTexture => outputTexture;

        protected bool isDebug = false;
        public bool IsDebug
        {
            get => isDebug;
            set
            {
                if (isDebug == value)
                    return;

                isDebug = value;
                if (isDebug)
                {
                    CreateRenderTexture();
                }
                else
                {
                    DeleteRenderTexture();
                }
            }
        }

        private void Awake()
        {
            DeleteRenderTexture();
            isDebug = false;
        }

        private void OnDestroy()
        {
            DeleteRenderTexture();
        }

        private void DeleteRenderTexture()
        {
            outputTexture = null;
        }

        private void CreateRenderTexture()
        {
            outputTexture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
            outputTexture.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;
            outputTexture.depthStencilFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.D32_SFloat;
            outputTexture.autoGenerateMips = true;
            outputTexture.Create();
        }

        private void OnGUI()
        {
            if (outputTexture != null)
                Graphics.Blit(null, outputTexture);
        }
    }
}