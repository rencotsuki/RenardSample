using System;
using UnityEngine.Playables;
using Renard.Debuger;

namespace SignageHADO.Timeline
{
    [Serializable]
    public abstract class PlayableBehaviourCustom : PlayableBehaviour
    {
        /*
         * 例: GameObjectを設定して操作
         * public GameObject charaObj = null;
         * private Vector3 startPos = new Vector3( 0, 0, -5 );
         * private Vector3 endPos = new Vector3( 0, 0, 0 );
         */

        public bool IsDebugLog = false;

        protected void Log(DebugerLogType logType, string methodName, string message)
        {
            if (!IsDebugLog)
            {
                if (logType == DebugerLogType.Info)
                    return;
            }

            DebugLogger.Log(this.GetType(), logType, methodName, message);

            OutputLog($"[{this.GetType()}] - (<color=green>{methodName}</color>): {message}");
        }

        protected virtual void OutputLog(string message)
        {
            // 何もしない
        }

        /*
         * 例: GameObjectを設定して操作
         * // PlayableTrack再生時フレーム毎に呼ばれる
         * public override void PrepareFrame(Playable playable, FrameData info)
         * {
         *     if (charaObj == null) return;
         *     var currentTime = (float)playable.GetTime() / (float)playable.GetDuration();
         *     var currentPos = Vector3.Lerp(startPos, endPos, currentTime);
         *     charaObj.transform.localPosition = currentPos;
         * }
         */
    }
}
