using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using uLipSync;

namespace SignageHADO
{
    [Serializable]
    public class LipSyncScript : MonoBehaviourCustom
    {
        [SerializeField] private AudioSource audioSource = null;

        [Tooltip("Buffer time for reflecting Microphone input as AudioSource (too low can cause noise)")]
        [SerializeField, Range(0.01f, 0.2f)] private float _bufferTime = 0.03f;
        [Tooltip("Threshold time to resynchronize Microphone and AudioSource")]
        [SerializeField, Range(0.01f, 0.2f)] private float _latencyTolerance = 0.05f;

        protected bool isPlaying => audioSource != null ? audioSource.isPlaying : false;
        protected AudioClip clip
        {
            get => audioSource != null ? audioSource.clip : null;
            private set
            {
                if (audioSource != null)
                    audioSource.clip = value;
            }
        }
        protected float sourceTime
        {
            get => audioSource != null ? audioSource.time : 0f;
            private set
            {
                if (audioSource != null)
                    audioSource.time = value;
            }
        }

        private float lipSuncLowPassFilter => 0.125f;
        private float lipSuncHighPassFilter => 0.5f;
        private float lipSuncLerp => 0.85f;

        [SerializeField] protected string micDeviceName = default;

        [HideInInspector] private int _minFreq = 0;
        [HideInInspector] private int _maxFreq = 0;
        [HideInInspector] private AudioClip _micClip = null;
        [HideInInspector] private bool _isRecording = false;
        [HideInInspector] private bool _isStartRequested = false;
        [HideInInspector] private bool _isStopRequested = false;

        private int maxRetryMilliSec => 1000;

        protected bool isMicClipSet => clip != null && _micClip != null ? (clip == _micClip) : false;

        [HideInInspector] private float _micTime = 0f;
        [HideInInspector] private float _latency = 0f;

        public float LipSuncAa = 0f;
        public float LipSuncIh = 0f;
        public float LipSuncOu = 0f;
        public float LipSuncEe = 0f;
        public float LipSuncOh = 0f;

        [HideInInspector] private int _index = 0;
        [HideInInspector] private float _lipSuncWeight = 0f;
        [HideInInspector] private float _lipSuncValue = 0f;
        [HideInInspector] private float _lipSuncWeightBefore = 0f;

        private CancellationTokenSource _onUpdateToken = null;

        private void OnDestroy()
        {
            OnDisposeUpdate();
        }

        private void OnDisposeUpdate()
        {
            _onUpdateToken?.Dispose();
            _onUpdateToken = null;
        }

        private void OnSetupUpdate()
        {
            OnDisposeUpdate();
            _onUpdateToken = new CancellationTokenSource();
            OnUpdateAsync(_onUpdateToken.Token).Forget();
        }

        private async UniTask OnUpdateAsync(CancellationToken token)
        {
            OnStartRecord();

            try
            {
                while (_onUpdateToken != null)
                {
                    await OnUpdateDeviceInAsync(token);

                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                    token.ThrowIfCancellationRequested();
                }
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnUpdateAsync", $"{ex.Message}");
            }
            finally
            {
                OnStopRecord();
            }
        }

        public void PlayVoice(AudioClip clip, float volume = 1f, bool loop = false)
        {
            if (audioSource == null || clip == null)
                return;

            StopVoice();

            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.loop = loop;

            audioSource.Play();
        }

        public void StopVoice()
        {
            audioSource?.Stop();
        }

        /// <summary>※通信による連動を想定した反映処理</summary>
        public void UpdateLipSyncUpdate(LipSyncInfo info)
        {
            // TODO: 何かしらのフィルターをかけなければ...。

            UpdateLipSuncWeight(ref LipSuncAa, info.phonemeRatios, "A");
            UpdateLipSuncWeight(ref LipSuncIh, info.phonemeRatios, "I", "S");
            UpdateLipSuncWeight(ref LipSuncOu, info.phonemeRatios, "U");
            UpdateLipSuncWeight(ref LipSuncEe, info.phonemeRatios, "E");
            UpdateLipSuncWeight(ref LipSuncOh, info.phonemeRatios, "O");
        }

        protected void UpdateLipSuncWeight(ref float target, Dictionary<string, float> phonemeRatios, params string[] phonemeKeys)
        {
            _lipSuncWeight = 0f;
            _lipSuncWeightBefore = target;

            if (phonemeKeys != null && phonemeKeys.Length > 0)
            {
                _lipSuncWeight = GetLipSuncWeight(phonemeRatios, phonemeKeys[0]);

                for (_index = 1; _index < phonemeKeys.Length; _index++)
                {
                    _lipSuncValue = GetLipSuncWeight(phonemeRatios, phonemeKeys[_index]);

                    if (_lipSuncValue <= lipSuncLowPassFilter)
                        continue;

                    _lipSuncWeight += _lipSuncValue;
                    _lipSuncWeight *= 0.5f;
                }

                _lipSuncWeight = Mathf.Floor(_lipSuncWeight * 100) * 0.01f;
            }

            target = ToLerp(_lipSuncWeightBefore, _lipSuncWeight, lipSuncLerp);
        }

        protected float GetLipSuncWeight(Dictionary<string, float> phonemeRatios, string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                if (phonemeRatios != null && phonemeRatios.Count > 0)
                {
                    if (phonemeRatios.ContainsKey(key))
                    {
                        if (phonemeRatios[key] > lipSuncLowPassFilter)
                            return phonemeRatios[key] > lipSuncHighPassFilter ? lipSuncHighPassFilter : phonemeRatios[key];
                    }
                }
            }
            return 0f;
        }

        protected float ToLerp(float start, float end, float lerp)
            => start + (end - start) * Mathf.Clamp01(lerp);

        #region Microphone

        /// <summary>デバイス設定</summary>
        public void SetupDevice(string deviceName)
        {
            OnStartRecordInternal(deviceName);
        }

        /// <summary>解除</summary>
        public void RemoveDevice()
        {
            OnStopRecordInternal();
        }

        private void OnStartRecord()
        {
            _isStartRequested = true;
            _isStopRequested = false;
        }

        private void OnStopRecord()
        {
            OnStopRecordInternal();
        }

        protected async UniTask OnUpdateDeviceInAsync(CancellationToken token)
        {
            try
            {
                UpdateDevice();

                if (_isRecording && !isMicClipSet)
                {
                    OnStopRecordInternal();
                    _micClip = null;
                }

                if (_isStartRequested)
                {
                    _isStartRequested = false;
                    await OnStartRecordInternalAsync(token);
                }

                if (_isStopRequested)
                {
                    _isStopRequested = false;
                    OnStopRecordInternal();
                }

                UpdateLatencyCheck();
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnUpdateDeviceInAsync", $"{ex.Message}");

                OnStopRecordInternal();
            }
        }

        private void UpdateDevice()
        {
            if (string.IsNullOrEmpty(micDeviceName))
                OnStopRecordInternal();

            if (_isRecording)
                OnStartRecord();
        }

        private void UpdateLatencyCheck()
        {
            if (!_isRecording)
                return;

#if !UNITY_WEBGL || UNITY_EDITOR
            _micTime = (Microphone.GetPosition(micDeviceName) / _maxFreq);
            _latency = _micTime - sourceTime;

            if (_latency < -clip.length / 2)
                _latency += clip.length;

            if (Mathf.Abs(_latency) > _latencyTolerance + _bufferTime)
            {
                if (Microphone.IsRecording(micDeviceName))
                {
                    sourceTime = _micTime - _bufferTime;
                }
                else
                {
                    OnStartRecord();
                }
            }
#endif
        }

        private void OnStartRecordInternal(string deviceName)
        {
            OnStopRecordInternal();

            try
            {
                if (string.IsNullOrEmpty(deviceName))
                    throw new Exception("microphone not select.");

                micDeviceName = deviceName;

#if !UNITY_WEBGL || UNITY_EDITOR
                Microphone.GetDeviceCaps(micDeviceName, out _minFreq, out _maxFreq);
#endif

                if (_maxFreq <= 0)
                    _maxFreq = 48000;

                OnSetupUpdate();
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnStartRecordInternal", $"{ex.Message}");

                micDeviceName = string.Empty;
            }
        }

        private async UniTask OnStartRecordInternalAsync(CancellationToken token)
        {
            if (audioSource == null)
                return;

#if !UNITY_WEBGL || UNITY_EDITOR
            _micClip = Microphone.Start(micDeviceName, true, 10, _maxFreq);
            clip = _micClip;

            var retryCount = 0;

            while (Microphone.GetPosition(micDeviceName) <= 0)
            {
                if (++retryCount >= maxRetryMilliSec)
                {
                    Log(DebugerLogType.Info, "OnStartRecordInternalAsync", "Failed to get microphone.");
                    return;
                }

                await UniTask.Delay(1, cancellationToken: token);
                token.ThrowIfCancellationRequested();
            }

            audioSource.loop = true;
            audioSource.Play();

            _isRecording = true;
#endif
        }

        private void OnStopRecordInternal()
        {
            if (isPlaying && isMicClipSet)
                audioSource?.Stop();

#if !UNITY_WEBGL || UNITY_EDITOR
            if (!string.IsNullOrEmpty(micDeviceName))
                Microphone.End(micDeviceName);
#endif

            _isRecording = false;
        }

        #endregion
    }
}
