using System;

/// <summary>センサーアクション識別</summary>
[Flags]
public enum SensorActionEnum : int
{
    None = 0,

    /// <summary>シェイク</summary>
    Shake = 1 << 0,

    /// <summary>スキル</summary>
    Skill = 1 << 1
}

/// <summary>腕センサーの向き</summary>
public enum SensorArmDirectionEnum : int
{
    None = 0,

    /// <summary>上向き</summary>
    Up = 1,

    /// <summary>下向き</summary>
    Down = 2
}

public static class SensorEnumExtensions
{
    public static SensorActionEnum SetTo(this SensorActionEnum target, SensorActionEnum value)
    {
        return (target = value);
    }

    public static SensorActionEnum AddTo(this SensorActionEnum target, params SensorActionEnum[] values)
    {
        foreach (var item in values)
        {
            target = target | item;
        }
        return target;
    }

    public static SensorActionEnum RemoveTo(this SensorActionEnum target, params SensorActionEnum[] values)
    {
        foreach (var item in values)
        {
            target = target & ~item;
        }
        return target;
    }

    public static bool IsInclude(this SensorActionEnum target, SensorActionEnum value)
    {
        return (target & value) != 0;
    }
}
