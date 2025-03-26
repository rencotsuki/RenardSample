using UnityEngine;

public static class CoodinateTransformer
{
    public static int LandscapeRotator(ScreenOrientation orientation)
        => (orientation == ScreenOrientation.LandscapeLeft || orientation == ScreenOrientation.LandscapeLeft) ? 1 : -1;

    public static Vector3 LandscapeLeftToUnity(Vector3 vec)
        => Application.platform == RuntimePlatform.IPhonePlayer ? new Vector3(vec.x, vec.y, -vec.z) : vec;

    public static Quaternion LandscapeLeftToUnity(Quaternion rot)
        => Application.platform == RuntimePlatform.IPhonePlayer ? new Quaternion(-rot.x, -rot.y, rot.z, -rot.w) : rot;

    public static Vector3 LandscapeRightToUnity(Vector3 vec)
        => Application.platform == RuntimePlatform.IPhonePlayer ? new Vector3(vec.x, vec.y, -vec.z) : vec;

    public static Quaternion LandscapeRightToUnity(Quaternion rot)
        => Application.platform == RuntimePlatform.IPhonePlayer ? new Quaternion(-rot.x, -rot.y, rot.z, -rot.w) : rot;

    public static Vector3 WitMotionToUnity(Vector3 vec) => new Vector3(vec.x, vec.y, -vec.z); // 方向の調整が必要ならココで変換する

    public static Quaternion WitMotionToUnity(Quaternion rot) => new Quaternion(-rot.x, rot.z, -rot.y, -rot.w); // 方向の調整が必要ならココで変換する

    private static Vector3 FrontVector(Vector3 gravity) => Quaternion.AngleAxis(-90f, Vector3.up) * gravity;
    public static float unityToRealWorldFrontComponent(Vector3 acceleration, Vector3 gravity)
        => gravity.z < 0 ? -Vector3.Dot(FrontVector(gravity), acceleration) : Vector3.Dot(FrontVector(gravity), acceleration);
}
