using System;
using System.ComponentModel;
using UnityEngine.Playables;

namespace SignageHADO.Timeline
{
    using Game;

    [Serializable]
    [DisplayName("GameStatus")]
    public class GameStatusPlayableBehaviour : PlayableBehaviourCustom
    {
        protected GameStatus gameStatus = GameStatus.None;
        protected bool finishEsc = false;

        protected GameManager manager => GameManager.Singleton;

        public void Settings(GameStatus gameStatus, bool finishEsc)
        {
            this.gameStatus = gameStatus;
            this.finishEsc = finishEsc;
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            manager?.SetGameStatus(gameStatus);
        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            if (gameStatus == GameStatus.Playing ||
                gameStatus == GameStatus.Result)
            {
                // ミリ秒を秒に変換して送る
                manager?.UpdateStatusTimer((float)playable.GetTime());
            }
            else
            {
                manager?.UpdateStatusTimer(0f);
            }
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (gameStatus == GameStatus.Playing ||
                gameStatus == GameStatus.End ||
                gameStatus == GameStatus.Result)
            {
                if (finishEsc)
                    manager?.Esc();
            }
        }
    }
}
