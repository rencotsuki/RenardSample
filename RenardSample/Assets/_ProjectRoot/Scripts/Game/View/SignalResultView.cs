using System;
using UnityEngine;
using UnityEngine.UI;

namespace SignageHADO
{
    /*
     * 一旦は一連動作で組む
     * 綺麗にやるならパーツ・項目ごとの表示トリガーにする方がいいと思う
     * それをやるならタイムラインとゲームマネージャー側を考慮する必要がある
     * 
     * アニメーション設定
     * 描画時間 ※トータルを１にすると指定秒数以内に収まる
     */

    [Serializable]
    public class SignalResultView : MonoBehaviourCustom
    {
        [SerializeField] private TimelineAnimationHandler timelineAnimationSignal = default;
        [SerializeField] private TimelineAnimationHandler timelineAnimationResult = default;
        [Header("Animation")]
        [SerializeField] private AnimationClip signalHide = default;
        [SerializeField] private AnimationClip signalClose = default;
        [SerializeField] private AnimationClip resultHide = default;
        [SerializeField] private AnimationClip resultClose = default;
        [Header("-- Rank")]
        [SerializeField] private Image imgRankS = default;
        [SerializeField] private Image imgRankA = default;
        [SerializeField] private Image imgRankB = default;
        [SerializeField] private Image imgRankC = default;
        [Header("-- Comment")]
        [SerializeField] private Image imgCommentator = default;
        [SerializeField] private Image imgCommentS = default;
        [SerializeField] private Image imgCommentA = default;
        [SerializeField] private Image imgCommentB = default;
        [SerializeField] private Image imgCommentC = default;
        [Header("-- Kill")]
        [Header("桁の大きい順")]
        [SerializeField] private Image[] imgKillCounts = default;
        [Header("桁の大きい順")]
        [SerializeField] private Image[] imgKillPoints = default;
        [Header("-- Combo")]
        [Header("桁の大きい順")]
        [SerializeField] private Image[] imgComboCounts = default;
        [Header("桁の大きい順")]
        [SerializeField] private Image[] imgComboPoints = default;
        [Header("-- TotalPoint")]
        [Header("桁の大きい順")]
        [SerializeField] private Image[] imgTotalPoints = default;
        [Header("-- Font")]
        [Header("0～9の順")]
        [SerializeField] private Sprite[] spriteCount = default;
        [Header("0～9の順")]
        [SerializeField] private Sprite[] spritePoint = default;

        public TimelineAnimationHandler HandlerSignal => timelineAnimationSignal;
        public TimelineAnimationHandler HandlerResult => timelineAnimationResult;

        private void Start()
        {
            ResetView();
        }

        public void ResetView()
        {
            timelineAnimationSignal?.Play(signalHide);
            timelineAnimationResult?.Play(resultHide);
        }

        public void CloseSignal()
        {
            timelineAnimationSignal?.Play(signalClose);
        }

        public void CloseResult()
        {
            timelineAnimationResult?.Play(resultClose);
        }

        public void SetResultScore(GameScore score)
        {
            // 桁の高い順に！
            SetNumberSprite(score.BreakEnemy, spriteCount, imgKillCounts);
            SetNumberSprite(score.MaxCombo, spriteCount, imgComboCounts);

            SetNumberSprite(score.KillBonus, spritePoint, imgKillPoints);
            SetNumberSprite(score.ComboBonus, spritePoint, imgComboPoints);
            SetNumberSprite(score.TotalPoint, spritePoint, imgTotalPoints);

            imgRankS.enabled = (score.Rank == GameScoreRank.S);
            imgRankA.enabled = (score.Rank == GameScoreRank.A);
            imgRankB.enabled = (score.Rank == GameScoreRank.B);
            imgRankC.enabled = (score.Rank == GameScoreRank.C);

            imgCommentS.enabled = (score.Rank == GameScoreRank.S);
            imgCommentA.enabled = (score.Rank == GameScoreRank.A);
            imgCommentB.enabled = (score.Rank == GameScoreRank.B);
            imgCommentC.enabled = (score.Rank == GameScoreRank.C);
        }

        private void SetNumberSprite(int value, Sprite[] sprites, params Image[] images)
        {
            var number = value;
            var pointIndex = new int[images.Length];

            var headIndex = pointIndex.Length - 1;
            for (int i = pointIndex.Length - 1; i >= 0; i--)
            {
                pointIndex[i] = number % 10;
                number /= 10;

                if (pointIndex[i] > 0 && headIndex > i)
                    headIndex = i;
            }

            for (int i = 0; i < images.Length; i++)
            {
                images[i].sprite = sprites[pointIndex[i]];
                images[i].gameObject.SetActive((headIndex <= i));
            }
        }
    }
}
