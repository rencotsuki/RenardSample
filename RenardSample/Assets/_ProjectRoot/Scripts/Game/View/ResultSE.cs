using System;
using UnityEngine;

namespace SignageHADO
{
    /*
     * Animation EventからSEを鳴らすためのスクリプト
     */

    [Serializable]
    public class ResultSE : MonoBehaviourCustom
    {
        [SerializeField] private AudioClip seClipFrameIn = default;
        [SerializeField] private AudioClip seClipScore = default;
        [SerializeField] private AudioClip seClipFinale = default;

        public void PlaySEFrameIn()
            => SoundHandler.PlaySE(seClipFrameIn);

        public void PlaySEScore()
            => SoundHandler.PlaySE(seClipScore);

        public void PlaySEFinale()
            => SoundHandler.PlaySE(seClipFinale);
    }
}
