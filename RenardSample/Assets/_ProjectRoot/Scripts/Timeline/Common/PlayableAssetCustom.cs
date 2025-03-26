using System;
using UnityEngine;
using UnityEngine.Playables;
using Renard.Debuger;

namespace SignageHADO.Timeline
{
    [Serializable]
    public abstract class PlayableAssetCustom<T> : PlayableAsset where T : PlayableBehaviourCustom, new()
    {
        /*
         * 例: GameObjectを設定して操作
         * [SerializeField] protected ExposedReference<GameObject> charaObj = default;
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

        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var behaviour = new T();

            OnCreatePlayable(ref behaviour, graph, go);

            return ScriptPlayable<T>.Create(graph, behaviour);
        }

        protected virtual void OnCreatePlayable(ref T playableBehaviour, PlayableGraph graph, GameObject go)
        {
            // playableBehaviourに対して色々な設定を行う

            /*
             * 例: GameObjectを設定して操作
             * behaviour.charaObj = charaObj.Resolve( graph.GetResolver() );
             */
        }
    }
}
