using System;
using System.ComponentModel;
using UnityEngine.Playables;

namespace SignageHADO.Timeline
{
    using Game;

    [Serializable]
    [DisplayName("Enemy")]
    public class EnemyPlayableBehaviour : PlayableBehaviourCustom
    {
        protected int emenyNo = 0;
        protected EmenyAction emenyAction = null;
        protected float duration = 0f;

        protected GameManager manager => GameManager.Singleton;
        protected GameStatus gameStatus => manager != null ? manager.Status : GameStatus.None;

        private EnemyObject _enemyObject = null;
        private float _runWait = 0f;

        public void Settings(int emenyNo, EmenyAction emenyAction, float duration, float runWait)
        {
            this.emenyNo = emenyNo;
            this.emenyAction = emenyAction;
            this.duration = duration;
            _runWait = runWait;
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            _enemyObject = manager != null ? manager.SpawnEnemy(emenyNo, emenyAction, duration, _runWait) : null;
        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            _enemyObject?.UpdateStatus(gameStatus, (float)playable.GetTime());
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            _enemyObject?.Return();
        }
    }
}
