using System;
using UnityEngine;
using Unity.Cinemachine;
using UniRx;
using SignageHADO.Game;

public enum GameStatus
{
    None = 0,

    /// <summary>待機</summary>
    Entry,

    /// <summary>開始</summary>
    Start,

    /// <summary>プレイ中</summary>
    Playing,

    /// <summary>終了</summary>
    End,

    /// <summary>リザルト</summary>
    Result,
}

public enum GameScoreRank : int
{
    S, A, B, C
}

[Serializable]
public struct GameScore
{
    public int BreakEnemy;
    public int LeftShot;
    public int RightShot;
    public int Hit;
    public int MaxCombo;

    public int Shot => LeftShot + RightShot;
    public float HitRate => Shot > 0 ? (float)Hit / (float)Shot : 0f;

    public int Point;
    public int ComboBonus;
    public int KillBonus;

    public int TotalPoint => Point + ComboBonus + KillBonus;

    public GameScoreRank Rank;

    public void Reset()
    {
        BreakEnemy = 0;
        LeftShot = 0;
        RightShot = 0;
        Hit = 0;
        MaxCombo = 0;

        Point = 0;
        ComboBonus = 0;
        KillBonus = 0;
        Rank = GameScoreRank.C;
    }
}

public class GameHandler
{
    private static GameManager manager => GameManager.Singleton;

    public const bool HitDirectionWait = true; //TODO: 当て感という仕様の実装
    public const float HitDirectionWaitTime = 2f / 60f; //TODO: 当て感という仕様の実装

    public static bool Init
        => manager != null ? manager.Init : false;

    public static Vector3 WorldAnchorPos => manager != null ? manager.WorldAnchorPos : Vector3.zero;

    public static float WorldScale => manager != null ? manager.WorldScale : 1f;

    public static Camera ViewCamera => SystemHandler.DisplayCamera;

    public static CinemachineCamera PlayerCamera
        => manager != null ? manager.PlayerCamera : null;

    public static GameStatus GameStatus
        => manager != null ? manager.Status : GameStatus.None;

    public static float GameTime
        => manager != null ? manager.GameTime : 0f;

    public static float FillAmountGameTime
        => manager != null ? manager.FillAmountGameTime : 0f;

    public static float ResultTime
        => manager != null ? manager.ResultTime : 0f;

    public static float FillAmountResultTime
        => manager != null ? manager.FillAmountResultTime : 0f;

    public static int Point
        => manager != null ? manager.Point : 0;

    public static int ComboCount
        => manager != null ? manager.ComboCount : 0;

    public static int BreakEnemy
        => manager != null ? manager.BreakEnemy : 0;

    public static IObservable<Unit> OnEntryObservable
        => manager != null ? manager.OnEntrySubject : default;

    public static IObservable<Unit> OnStartObservable
    => manager != null ? manager.OnStartSubject : default;

    public static IObservable<Unit> OnPlayObservable
        => manager != null ? manager.OnPlaySubject : default;

    public static IObservable<Unit> OnEndObservable
        => manager != null ? manager.OnEndSubject : default;

    public static IObservable<Unit> OnEscObservable
        => manager != null ? manager.OnEscSubject : default;

    public static IObservable<GameScore> ReportGameResultObservable
        => manager != null ? manager.ReportGameResultSubject : default;

    public static void SetWorldScale(float scale)
        => manager?.SetWorldScale(scale);

    public static void SetGameResultTime(float gameTime, float resultTime)
        => manager?.SetGameResultTime(gameTime, resultTime);

    public static void MissShot()
        => manager?.MissShot();

    #region Player

    public static bool KeepShotAction => manager != null ? manager.KeepShotAction : false;

    public static bool ActiveManualAction => manager != null ? manager.ActiveManualAction : false;

    public static void ShotBullet()
        => manager?.ShotBullet();

    public static void SetKeepShotAction(bool value)
        => manager?.SetKeepShotAction(value);

    public static void ActiveManualControl(bool active)
        => manager?.ActiveManualControl(active);

    public static void SetManualControl(float vertical, float horizontal)
        => manager?.SetManualControl(vertical, horizontal);

    #endregion

    #region Bullet

    public const float BulletSpeed = 10.5f; //ﾊﾟﾗﾒﾀ=5の数値

    public const float BulletScale = 0.52f; //ﾊﾟﾗﾒﾀ=5の数値

    public static void ShotBullet(int attack, Vector3 pos, Vector3 direct, bool right)
        => manager?.ShotBullet(attack, pos, direct, right);

    public static void ReturnBullet(BulletScript bullet)
        => manager?.ReturnBullet(bullet);

    #endregion

    #region Enemy

    public static void ReturnEnemy(EnemyObject enemy)
        => manager?.ReturnEnemy(enemy);

    #endregion
}
