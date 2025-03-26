/*
 * 参考：
 * 【Unity】AnimationControllerを用意せずAnimatorによるアニメーションを使う
 * https://tsubakit1.hateblo.jp/entry/2017/08/02/235736
 */

using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

namespace SignageHADO
{
    [RequireComponent(typeof(Animator))]
    [DisallowMultipleComponent]
    public class TimelineMotionHandler : MonoBehaviourCustom
    {
        [SerializeField] private float defaultFadeLength = 0.3f;
        [SerializeField] private bool footIK = true;

        public List<AnimationClip> clipList = default;

        protected PlayableGraph graph = default;
        protected AnimationMixerPlayable mixer = default;
        protected AnimationClipPlayable currentPlayable = default;
        protected AnimationClipPlayable prePlayable = default;
        protected AnimationClip resetPoseClip = null;
        protected AnimationClip currentClip = null;

        [HideInInspector] private float _tmpBeginTime = 0f;
        [HideInInspector] private float _tmpElapsedTime = 0f;
        [HideInInspector] private float _tmpSlerp = 1f;
        [HideInInspector] private Vector3 _tmpPosition = Vector3.zero;
        [HideInInspector] private Quaternion _tmpRotation = Quaternion.identity;

        [HideInInspector] private float _tmpWaitTime = 0f;
        [HideInInspector] private float _tmpDiff = 0f;
        [HideInInspector] private float _tmpRate = 0f;

        private CancellationTokenSource _onPlayAnimationToken = null;
        private CancellationTokenSource _onResetPositionToken = null;

        private void Awake()
        {
            graph = PlayableGraph.Create(name);
            AnimationPlayableOutput.Create(graph, name, GetComponent<Animator>());
        }

        private void OnDestroy()
        {
            OnDisposeResetPosition();
            graph.Destroy();
        }

        public void Setup(AnimationClip resetPose)
        {
            resetPoseClip = resetPose;
            clipList?.Add(resetPoseClip);

            mixer = AnimationMixerPlayable.Create(graph, 2);

            var output = graph.GetOutput(0);
            output.SetSourcePlayable(mixer);
            if (clipList.Count != 0)
            {
                currentPlayable = AnimationClipPlayable.Create(graph, clipList[0]);
                mixer.ConnectInput(0, currentPlayable, 0);
                mixer.SetInputWeight(0, 1);
                graph.Play();
            }
        }

        private void OnDisposeResetPosition()
        {
            _onResetPositionToken?.Dispose();
            _onResetPositionToken = null;
        }

        public void ResetPose(float duration, AnimationClip nextClip = null)
        {
            OnDisposeResetPosition();
            _onResetPositionToken = new CancellationTokenSource();
            OnResetPoseAsync(_onResetPositionToken.Token, duration, nextClip).Forget();
        }

        private async UniTask OnResetPoseAsync(CancellationToken token, float duration, AnimationClip nextClip)
        {
            try
            {
                if (duration > 0)
                {
                    CrossFade(resetPoseClip, duration * 0.5f, false);

                    _tmpBeginTime = Time.realtimeSinceStartup;
                    _tmpElapsedTime = 0f;
                    _tmpSlerp = 1f;
                    _tmpPosition = Vector3.zero;
                    _tmpRotation = Quaternion.identity;

                    while (_onResetPositionToken != null)
                    {
                        _tmpElapsedTime = Time.realtimeSinceStartup - _tmpBeginTime;
                        if (_tmpElapsedTime >= duration * 0.5f)
                            break;

                        _tmpSlerp = Mathf.Clamp01(_tmpElapsedTime / (duration * 0.5f));
                        _tmpPosition = Vector3.Slerp(transform.localPosition, Vector3.zero, _tmpSlerp);
                        _tmpRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity, _tmpSlerp);

                        transform.SetLocalPositionAndRotation(_tmpPosition, _tmpRotation);

                        await UniTask.Yield(PlayerLoopTiming.Update, token);
                        token.ThrowIfCancellationRequested();
                    }
                }
                else
                {
                    Play(resetPoseClip, false);
                }
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnResetPoseAsync", $"{ex.Message}");

                if (token.IsCancellationRequested)
                    return;
            }

            try
            {
                transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                CrossFade(nextClip, duration * 0.5f, false);
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnResetPoseAsync", $"{ex.Message}");
            }
        }

        private void DisconnectPlayables()
        {
            try
            {
                graph.Disconnect(mixer, 0);
                graph.Disconnect(mixer, 1);
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "DisconnectPlayables", $"{ex.Message}");
            }
            finally
            {
                if (prePlayable.IsValid())
                    prePlayable.Destroy();
            }
        }

        private bool IsPlaying(AnimationClip clip)
        {
            if (currentClip != null && clip != null)
            {
                // 再生中と同じものは通さない
                if (currentClip.Equals(clip))
                    return true;
            }
            return false;
        }

        public void Play(string clipName, bool checkPlaying = true)
        {
            Play(clipList.Find((c) => c.name == clipName), checkPlaying);
        }

        public void Play(AnimationClip clip, bool checkPlaying = true)
        {
            if (checkPlaying && IsPlaying(clip))
                return;

            try
            {
                OnDisposePlayAnimation();

                if (currentPlayable.IsValid())
                    currentPlayable.Destroy();

                DisconnectPlayables();

                currentClip = clip;

                if (currentClip != null)
                {
                    currentPlayable = AnimationClipPlayable.Create(graph, currentClip);
                    mixer.ConnectInput(0, currentPlayable, 0);

                    mixer.SetInputWeight(1, 0);
                    mixer.SetInputWeight(0, 1);
                }
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "Play", $"{ex.Message}");
            }
        }

        public void CrossFade(string animation, float fadeLength, bool checkPlaying = true)
        {
            CrossFade(clipList.Find((c) => c.name == animation), fadeLength, checkPlaying);
        }

        public void CrossFade(string animation, bool checkPlaying = true)
        {
            CrossFade(animation, defaultFadeLength, checkPlaying);
        }

        public void CrossFade(AnimationClip clip, bool checkPlaying = true)
        {
            CrossFade(clip, defaultFadeLength, checkPlaying);
        }

        public void CrossFade(AnimationClip clip, float fadeLength, bool checkPlaying = true)
        {
            if (checkPlaying && IsPlaying(clip))
                return;

            DisconnectPlayables();

            currentClip = clip;

            if (currentClip == null)
                return;

            OnDisposePlayAnimation();
            _onPlayAnimationToken = new CancellationTokenSource();
            OnPlayAnimationAsync(_onPlayAnimationToken.Token, fadeLength).Forget();
        }

        private void OnDisposePlayAnimation()
        {
            _onPlayAnimationToken?.Dispose();
            _onPlayAnimationToken = null;
        }

        private async UniTask OnPlayAnimationAsync(CancellationToken token, float transitionTime)
        {
            try
            {
                prePlayable = currentPlayable;
                prePlayable.SetApplyFootIK(footIK);
                currentPlayable = AnimationClipPlayable.Create(graph, currentClip);
                currentPlayable.SetApplyFootIK(footIK);
                mixer.ConnectInput(1, prePlayable, 0);
                mixer.ConnectInput(0, currentPlayable, 0);

                // 指定時間でアニメーションをブレンド
                _tmpWaitTime = Time.timeSinceLevelLoad + transitionTime;

                while (_onPlayAnimationToken != null)
                {
                    _tmpDiff = _tmpWaitTime - Time.timeSinceLevelLoad;
                    if (_tmpDiff <= 0)
                    {
                        mixer.SetInputWeight(1, 0);
                        mixer.SetInputWeight(0, 1);
                        break;
                    }
                    else
                    {
                        _tmpRate = Mathf.Clamp01(_tmpDiff / transitionTime);
                        mixer.SetInputWeight(1, _tmpRate);
                        mixer.SetInputWeight(0, 1 - _tmpRate);
                    }

                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                    token.ThrowIfCancellationRequested();
                }
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnPlayAnimationAsync", $"{ex.Message}");
            }
        }
    }
}
