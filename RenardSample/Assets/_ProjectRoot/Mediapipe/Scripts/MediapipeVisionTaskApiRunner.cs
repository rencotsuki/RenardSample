/*
 * Mediapipe - VisionTaskApiRunnerの改修
 */

using System.Collections;
using UnityEngine;
using Mediapipe.Unity;
using Mediapipe.Tasks.Vision.FaceLandmarker;
using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Tasks.Vision.ObjectDetector;

namespace SignageHADO.Tracking
{
    public abstract class MediapipeVisionTaskApiRunner : MediapipeBaseRunner
    {
        [SerializeField] protected MediapipeScreen screen = default;

        public bool Active => init && !isPaused;

        private Coroutine _coroutine = null;
        protected FaceLandmarker faceLandmarker = null;
        protected HandLandmarker handLandmarker = null;
        protected ObjectDetector objectDetector = null;

        public RunningMode runningMode;

        public override void Play()
        {
            if (_coroutine != null)
            {
                Stop();
            }
            base.Play();
            _coroutine = StartCoroutine(Run());
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
            StopCoroutine(_coroutine);
            ImageSourceProvider.ImageSource.Stop();
            faceLandmarker?.Close();
            faceLandmarker = null;

            handLandmarker?.Close();
            handLandmarker = null;

            objectDetector?.Close();
            objectDetector = null;
        }

        protected abstract IEnumerator Run();

        protected static void SetupAnnotationController<T>(AnnotationController<T> annotationController, ImageSource imageSource, bool expectedToBeMirrored = false) where T : HierarchicalAnnotation
        {
            annotationController.isMirrored = expectedToBeMirrored;
            annotationController.imageSize = new Vector2Int(imageSource.textureWidth, imageSource.textureHeight);
        }
    }
}
