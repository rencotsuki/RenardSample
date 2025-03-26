using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;

namespace SignageHADO.Timeline
{
    [Serializable]
    [DisplayName("SignageUI")]
    public class SignageNamePlatePlayableBehaviour : PlayableBehaviourCustom
    {
        protected Sprite spritePlate = null;
        protected Sprite spriteName = null;
        protected Sprite spriteNameRole = null;

        private float showAnima = 0f;
        private float hideAnima = 0f;

        protected SystemManager manager => SystemManager.Singleton;

        public void Settings(Sprite spritePlate, Sprite spriteName, Sprite spriteNameRole, float showAnima, float hideAnima)
        {
            this.spritePlate = spritePlate;
            this.spriteName = spriteName;
            this.spriteNameRole = spriteNameRole;
            this.showAnima = showAnima;
            this.hideAnima = hideAnima;
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            manager?.ShowNamePlate(spritePlate, spriteName, spriteNameRole, showAnima);
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            manager?.HideNamePlate(hideAnima);
        }
    }
}
