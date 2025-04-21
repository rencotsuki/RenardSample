/*
 * Mediapipe - BaseRunnerの改修
 */

using System;
using System.Collections;
using UnityEngine;

using Stopwatch = System.Diagnostics.Stopwatch;

namespace SignageHADO.Tracking
{
    public abstract class MediapipeBaseRunner : MonoBehaviourCustom
    {
        private static readonly string _BootstrapName = nameof(MediapipeBootstrap);

        [SerializeField] private GameObject _bootstrapPrefab = null;

#pragma warning disable IDE1006
        // TODO: make it static
        protected virtual string TAG => GetType().Name;
#pragma warning restore IDE1006

        protected bool init = false;
        protected MediapipeBootstrap bootstrap = null;
        protected bool isPaused = false;

        private readonly Stopwatch _stopwatch = new();

        protected virtual IEnumerator Start()
        {
            init = false;

            bootstrap = FindBootstrap();
            yield return new WaitUntil(() => bootstrap.isFinished);

            init = bootstrap.isFinished;
            Play();
        }

        /// <summary>
        ///   Start the main program from the beginning.
        /// </summary>
        public virtual void Play()
        {
            isPaused = false;
            _stopwatch.Restart();
        }

        /// <summary>
        ///   Pause the main program.
        /// <summary>
        public virtual void Pause()
        {
            isPaused = true;
        }

        /// <summary>
        ///    Resume the main program.
        ///    If the main program has not begun, it'll do nothing.
        /// </summary>
        public virtual void Resume()
        {
            isPaused = false;
        }

        /// <summary>
        ///   Stops the main program.
        /// </summary>
        public virtual void Stop()
        {
            isPaused = true;
            _stopwatch.Stop();
        }

        protected long GetCurrentTimestampMillisec() => _stopwatch.IsRunning ? _stopwatch.ElapsedTicks / TimeSpan.TicksPerMillisecond : -1;

        protected MediapipeBootstrap FindBootstrap()
        {
            var bootstrapObj = GameObject.Find(_BootstrapName);

            if (bootstrapObj == null)
            {
                Log(DebugerLogType.Info, "FindBootstrap", "Initializing the Bootstrap GameObject");
                bootstrapObj = Instantiate(_bootstrapPrefab);
                bootstrapObj.name = _BootstrapName;
                DontDestroyOnLoad(bootstrapObj);
            }

            return bootstrapObj.GetComponent<MediapipeBootstrap>();
        }
    }
}
