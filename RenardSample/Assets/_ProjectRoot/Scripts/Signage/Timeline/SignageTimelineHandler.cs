using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Unity.Cinemachine;
using SignageHADO.Timeline;

[RequireComponent(typeof(PlayableDirector))]
public class SignageTimelineHandler : TimelineHandler
{
    // 演出/進行
    protected TimelineDirectionData demoPart = new TimelineDirectionData(SignageStatus.DemoPart);
    protected TimelineDirectionData tutorialPart = new TimelineDirectionData(SignageStatus.TutorialPart);
    protected TimelineDirectionData gamePart = new TimelineDirectionData(SignageStatus.GamePart);
    protected TimelineDirectionData infoPart = new TimelineDirectionData(SignageStatus.InfoPart);

    public void Play(SignageStatus status)
    {
        if (status == SignageStatus.TutorialPart)
        {
            Play(tutorialPart);
        }
        else if (status == SignageStatus.GamePart)
        {
            Play(gamePart);
        }
        else if (status == SignageStatus.InfoPart)
        {
            Play(infoPart);
        }
        else
        {
            Play(demoPart);
        }
    }

    public void DebugPlay(PlayableAsset localPlayable, DirectorWrapMode wrapMode = DirectorWrapMode.Loop)
        => OnPlay(default, localPlayable, wrapMode);

    protected override async UniTask OnSetPlayableAssetsAsync(CancellationToken token, TimelineAsset timelineAsset)
    {
        SetCameraCinemachineShot(timelineAsset);

        SetGameStatusTrack(timelineAsset);

        if (SystemHandler.DrawCamera != null)
            BindObjectTimelineAsset(playableDirector, new [] { typeof(CinemachineTrack) }, SystemHandler.DrawCamera);
    }

    private void SetCameraCinemachineShot(TimelineAsset timeline)
    {
        try
        {
            if (timeline == null)
                throw new Exception("null timeline.");

            CinemachineShot noneCinemachineShot = null;
            foreach (var track in timeline.GetOutputTracks())
            {
                if (track.GetType() != typeof(CinemachineTrack)) continue;

                foreach (var clip in track.GetClips())
                {
                    if (clip.asset.GetType() != typeof(CinemachineShot)) continue;

                    if (GameHandler.PlayerCamera != null && clip.displayName == "PlayerCamera")
                    {
                        noneCinemachineShot = clip.asset as CinemachineShot;
                        if (noneCinemachineShot != null)
                            playableDirector.SetReferenceValue(noneCinemachineShot.VirtualCamera.exposedName, GameHandler.PlayerCamera);
                    }

                    if (SignageHandler.SignageCamera1 != null && clip.displayName == "SignageCamera1")
                    {
                        noneCinemachineShot = clip.asset as CinemachineShot;
                        if (noneCinemachineShot != null)
                            playableDirector.SetReferenceValue(noneCinemachineShot.VirtualCamera.exposedName, SignageHandler.SignageCamera1);
                    }

                    if (SignageHandler.SignageCamera2 != null && clip.displayName == "SignageCamera2")
                    {
                        noneCinemachineShot = clip.asset as CinemachineShot;
                        if (noneCinemachineShot != null)
                            playableDirector.SetReferenceValue(noneCinemachineShot.VirtualCamera.exposedName, SignageHandler.SignageCamera2);
                    }

                    if (SignageHandler.SignageCamera3 != null && clip.displayName == "SignageCamera3")
                    {
                        noneCinemachineShot = clip.asset as CinemachineShot;
                        if (noneCinemachineShot != null)
                            playableDirector.SetReferenceValue(noneCinemachineShot.VirtualCamera.exposedName, SignageHandler.SignageCamera3);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Log(DebugerLogType.Info, "SetCameraCinemachineShot", $"{ex.Message}");
        }
    }

    private void SetGameStatusTrack(TimelineAsset timeline)
    {
        try
        {
            if (timeline == null)
                throw new Exception("null timeline.");

            var exists = false;
            var gameTime = 0f;
            var resultTime = 0f;
            GameStatusPlayableAsset playableAsset = null;

            foreach (var track in timeline.GetOutputTracks())
            {
                if (track.GetType() != typeof(GameStatusTrack)) continue;

                foreach (var clip in track.GetClips())
                {
                    if (clip.asset.GetType() != typeof(GameStatusPlayableAsset)) continue;

                    playableAsset = clip.asset as GameStatusPlayableAsset;
                    if (playableAsset == null) continue;

                    if (playableAsset.GameStatus == GameStatus.Playing)
                    {
                        gameTime = (float)(Mathf.FloorToInt((float)clip.duration * 10f) * 0.1f);
                        exists = true;
                    }

                    if (playableAsset.GameStatus == GameStatus.Result)
                        resultTime = (float)(Mathf.FloorToInt((float)clip.duration * 10f) * 0.1f);
                }
            }

            if (exists)
                GameHandler.SetGameResultTime(gameTime, resultTime);
        }
        catch (Exception ex)
        {
            Log(DebugerLogType.Info, "SetGameStatusTrack", $"{ex.Message}");
        }
    }

    protected void BindObjectTimelineAsset<T>(PlayableDirector playableDirector, Type[] trackTypes, T bindObject, params string[] trackNames) where T : UnityEngine.Object
    {
        try
        {
            var outputTracks = (playableDirector.playableAsset as TimelineAsset)?.GetOutputTracks();

            foreach (TrackAsset trackAsset in outputTracks)
            {
                if (trackTypes.Length > 0)
                {
                    if (!trackTypes.Contains(trackAsset.GetType()))
                        continue;
                }

                if (trackNames.Length > 0)
                {
                    if (!trackNames.Contains(trackAsset.name))
                        continue;
                }

                foreach (PlayableBinding binding in trackAsset.outputs)
                {
                    if (binding.outputTargetType != typeof(T))
                        continue;

                    playableDirector.SetGenericBinding(binding.sourceObject, bindObject);
                }
            }
        }
        catch (Exception ex)
        {
            Log(DebugerLogType.Info, $"BindObjectTimelineAsset<{typeof(T).Name}>", $"{ex.Message}");
        }
    }
}
