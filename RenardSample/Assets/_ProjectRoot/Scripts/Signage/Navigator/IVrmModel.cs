using UnityEngine;

/// <summary>表情</summary>
public enum ExpressionEnum : int
{
    Neutral = 0,

    /// <summary>喜</summary>
    Happy,
    /// <summary>怒</summary>
    Angry,
    /// <summary>哀</summary>
    Sad,
    /// <summary>楽</summary>
    Relaxed,
    /// <summary>驚</summary>
    Surprised,

    /// <summary>目を閉じる</summary>
    Blink,
    /// <summary>ウィンク(左)</summary>
    WinkLeft,
    /// <summary>ウィンク(右)</summary>
    WinkRight,
}

public interface IVrmModel
{
    public float OriginScale();

    public float ScaleOffset();

    #region Body

    public Vector3 AnchorPos();
    public float AnchorScale();

    public Vector3 RootPos();
    public Quaternion RootRot();

    public Quaternion Spine();
    public Quaternion Chest();
    public Quaternion UpperChest();
    public Quaternion Neck();
    public Quaternion Head();

    public Quaternion Shoulder_Left();
    public Quaternion Arm_Left();
    public Quaternion Elbow_Left();
    public Quaternion Wrist_Left();

    public Quaternion Shoulder_Right();
    public Quaternion Arm_Right();
    public Quaternion Elbow_Right();
    public Quaternion Wrist_Right();

    public Quaternion Hip_Left();
    public Quaternion Knee_Left();
    public Quaternion Foot_Left();
    public Quaternion Toe_Left();

    public Quaternion Hip_Right();
    public Quaternion Knee_Right();
    public Quaternion Foot_Right();
    public Quaternion Toe_Right();

    #endregion

    #region Finger

    /// <summary>親指(左)</summary>
    public float ThumbFinger_Left();
    /// <summary>人差し指(左)</summary>
    public float IndexFinger_Left();
    /// <summary>中指(左)</summary>
    public float MiddleFinger_Left();
    /// <summary>薬指(左)</summary>
    public float RingFinger_Left();
    /// <summary>小指(左)</summary>
    public float LittleFinger_Left();

    /// <summary>親指(右)</summary>
    public float ThumbFinger_Right();
    /// <summary>人差し指(右)</summary>
    public float IndexFinger_Right();
    /// <summary>中指(右)</summary>
    public float MiddleFinger_Right();
    /// <summary>薬指(右)</summary>
    public float RingFinger_Right();
    /// <summary>小指(右)</summary>
    public float LittleFinger_Right();

    #endregion

    #region Face

    /// <summary>表情</summary>
    public int Expression();
    /// <summary>表情量</summary>
    public float ExpressionValue();
    /// <summary>リップ(Aa)</summary>
    public float Lip_Aa();
    /// <summary>リップ(Ih)</summary>
    public float Lip_Ih();
    /// <summary>リップ(Ou)</summary>
    public float Lip_Ou();
    /// <summary>リップ(Ee)</summary>
    public float Lip_Ee();
    /// <summary>リップ(Oh)</summary>
    public float Lip_Oh();

    #endregion
}
