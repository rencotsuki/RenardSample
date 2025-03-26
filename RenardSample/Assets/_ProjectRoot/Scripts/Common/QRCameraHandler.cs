using System;
using UnityEngine;

namespace SignageHADO
{
    [Serializable]
    public class QRCameraHandler
    {
        private WebCamTexture webcamTexture = null;

        ~QRCameraHandler()
        {
            Stop();
        }

        public WebCamTexture Setup(int width, int height, int fps = 30)
        {
            try
            {
                Stop();

                webcamTexture = new WebCamTexture(WebCamTexture.devices[0].name, width, height, fps);
                return webcamTexture;
            }
            catch
            {
                // 何もしない
            }
            return null;
        }

        public void Play() => webcamTexture?.Play();

        public void Stop()
        {
            webcamTexture?.Stop();
            webcamTexture = null;
        }
    }
}
