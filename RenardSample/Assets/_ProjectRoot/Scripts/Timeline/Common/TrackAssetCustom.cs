using System;
using UnityEngine.Timeline;
using Renard.Debuger;

namespace SignageHADO.Timeline
{
    [Serializable]
    public abstract class TrackAssetCustom : TrackAsset
    {
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
         * 例: TimelineのAsset表示名を変更する
         * protected override void OnCreateClip(TimelineClip clip)
         * {
         *     var asset = clip.asset as PlayableAssetCustom;
         *     clip.displayName = asset.[表示名の情報を拾ってくる]
         * }
         */
    }
}
