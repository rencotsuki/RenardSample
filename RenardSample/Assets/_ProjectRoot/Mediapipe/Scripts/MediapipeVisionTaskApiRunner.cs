/*
 * Mediapipe - VisionTaskApiRunnerの改修
 */

using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Mediapipe.Unity;

namespace SignageHADO.Tracking
{
    public abstract class MediapipeVisionTaskApiRunner : MediapipeBaseRunner
    {
        [SerializeField] protected MediapipeScreen screen = default;

        public bool Active => init && !isPaused;

        protected CancellationTokenSource onRunToken { get; private set; } = null;

        public RunningMode runningMode;

        private void OnDestroy()
        {
            OnDisposeRun();
        }

        public override void Play()
        {
            OnDisposeRun();

            base.Play();

            onRunToken = new CancellationTokenSource();
            Run(onRunToken.Token).Forget();
        }

        public override void Pause()
        {
            base.Pause();
            ImageSourceProvider.ImageSource.Pause();
        }

        public override void Resume()
        {
            base.Resume();
            var _ = StartCoroutine(ImageSourceProvider.ImageSource.Resume());
        }

        public override void Stop()
        {
            base.Stop();
            OnDisposeRun();
            ImageSourceProvider.ImageSource.Stop();
        }

        protected virtual void OnDisposeRun()
        {
            onRunToken?.Dispose();
            onRunToken = null;
        }

        protected abstract UniTask Run(CancellationToken token);

        protected static void SetupAnnotationController<T>(AnnotationController<T> annotationController, ImageSource imageSource, bool expectedToBeMirrored = false) where T : HierarchicalAnnotation
        {
            annotationController.isMirrored = expectedToBeMirrored;
            annotationController.imageSize = new Vector2Int(imageSource.textureWidth, imageSource.textureHeight);
        }
    }
}
