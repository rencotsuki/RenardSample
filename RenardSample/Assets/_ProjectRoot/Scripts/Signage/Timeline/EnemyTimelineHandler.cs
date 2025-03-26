using UnityEngine;
using UnityEngine.Playables;
using SignageHADO.Timeline;

[RequireComponent(typeof(PlayableDirector))]
public class EnemyTimelineHandler : TimelineHandler
{
    private PlayableAsset _data = default;

    public void SetData(PlayableAsset data)
    {
        _data = data;
    }

    public void Play()
        => OnPlay(default, _data, DirectorWrapMode.None);
}
