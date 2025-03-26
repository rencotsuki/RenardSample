/*
 * Mediapipe - Screenの改修
 */

using UnityEngine;
using UnityEngine.UI;
using Mediapipe.Unity;

using Experimental = Mediapipe.Unity.Experimental;

namespace SignageHADO.Tracking
{
    public class MediapipeScreen : MonoBehaviourCustom
    {
        [SerializeField] private RawImage _screen = null;

        private ImageSource _imageSource = null;

        public Texture texture
        {
            get => _screen != null ? _screen.texture : null;
            set
            {
                if (_screen != null)
                    _screen.texture = value;
            }
        }

        public Rect uvRect
        {
            set
            {
                if (_screen != null)
                    _screen.uvRect = value;
            }
        }

        protected Vector2Int imageSize = Vector2Int.zero;

        [HideInInspector] private Rect _resetUvRect = new Rect();
        [HideInInspector] private Rect _tmpRect = new Rect();
        [HideInInspector] private RotationAngle _tmpRotationAngle = new RotationAngle();

        public void Initialize(ImageSource imageSource)
        {
            _imageSource = imageSource;

            imageSize.x = _imageSource.textureWidth;
            imageSize.y = _imageSource.textureHeight;

            Resize(imageSize.x, imageSize.y);
            Rotate(_imageSource.rotation.Reverse());
            ResetUvRect(RunningMode.Async);
            texture = imageSource.GetCurrentTexture();
        }

        public void Resize(int width, int height)
        {
            if (_screen != null)
                _screen.rectTransform.sizeDelta = new Vector2(width, height);
        }

        public void Rotate(RotationAngle rotationAngle)
        {
            if (_screen != null)
                _screen.rectTransform.localEulerAngles = rotationAngle.GetEulerAngles();
        }

        public void ReadSync(Experimental.TextureFrame textureFrame)
        {
            if (!(texture is Texture2D))
            {
                texture = new Texture2D(imageSize.x, imageSize.y, TextureFormat.RGBA32, false);
                ResetUvRect(RunningMode.Sync);
            }

            textureFrame?.CopyTexture(texture);
        }

        private void ResetUvRect(RunningMode runningMode)
        {
            _resetUvRect.x = 0;
            _resetUvRect.y = 0;
            _resetUvRect.width = 1;
            _resetUvRect.height = 1;

            if (_imageSource.isVerticallyFlipped && runningMode == RunningMode.Async)
            {
                _resetUvRect = FlipVertically(_resetUvRect);
            }

            if (_imageSource.isFrontFacing)
            {
                _tmpRotationAngle = _imageSource.rotation;

                if (_tmpRotationAngle == RotationAngle.Rotation0 || _tmpRotationAngle == RotationAngle.Rotation180)
                {
                    _resetUvRect = FlipHorizontally(_resetUvRect);
                }
                else
                {
                    _resetUvRect = FlipVertically(_resetUvRect);
                }
            }

            uvRect = _resetUvRect;
        }

        private Rect FlipHorizontally(Rect rect)
        {
            _tmpRect.x = 1 - rect.x;
            _tmpRect.y = rect.y;
            _tmpRect.width = -rect.width;
            _tmpRect.height = rect.height;
            return _tmpRect;
        }

        private Rect FlipVertically(Rect rect)
        {
            _tmpRect.x = rect.x;
            _tmpRect.y = 1 - rect.y;
            _tmpRect.width = rect.width;
            _tmpRect.height = -rect.height;
            return _tmpRect;
        }
    }
}
