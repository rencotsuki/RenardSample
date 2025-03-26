/*
 * Mediapipe - LegacySolutionRunnerの改修
 */

using System.Collections;
using UnityEngine;
using Mediapipe.Unity;
using Mediapipe.Unity.Sample;

namespace SignageHADO.Tracking
{
    public abstract class MediapipeLegacySolutionRunner<TGraphRunner, TTask> : MediapipeBaseRunner where TGraphRunner : GraphRunner  where TTask : Mediapipe.Tasks.Vision.Core.BaseVisionTaskApi
    {
        [SerializeField] protected MediapipeScreen screen = null;
        [SerializeField] protected TGraphRunner graphRunner = null;

        public bool Active => init && !isPaused;

        protected Coroutine coroutine = null;
        protected TTask taskApi;

        public RunningMode runningMode = default;

        public long timeoutMillisec
        {
            get => graphRunner.timeoutMillisec;
            set => graphRunner.timeoutMillisec = value;
        }

        private void OnDestroy()
        {
            Stop();
        }

        public override void Play()
        {
            Stop();
            base.Play();
            coroutine = StartCoroutine(Run());
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

            try
            {
                if (coroutine != null)
                    StopCoroutine(coroutine);

                ImageSourceProvider.ImageSource?.Stop();
                graphRunner?.Stop();

                taskApi?.Close();
            }
            finally
            {
                coroutine = null;
                taskApi = null;
            }
        }

        protected abstract IEnumerator Run();

        protected static void SetupAnnotationController<T>(AnnotationController<T> annotationController, ImageSource imageSource, bool expectedToBeMirrored = false) where T : HierarchicalAnnotation
        {
            annotationController.isMirrored = expectedToBeMirrored ^ imageSource.isHorizontallyFlipped ^ imageSource.isFrontFacing;
            annotationController.rotationAngle = imageSource.rotation.Reverse();
        }

        protected static void SetupAnnotationControllerTask<T>(AnnotationController<T> annotationController, ImageSource imageSource, bool expectedToBeMirrored = false) where T : HierarchicalAnnotation
        {
            annotationController.isMirrored = expectedToBeMirrored;
            annotationController.imageSize = new Vector2Int(imageSource.textureWidth, imageSource.textureHeight);
        }
    }
}
