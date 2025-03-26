using UnityEngine;

public static class SensorArmDirection
{
    private static float lowerAngleX => -60f;
    private static float higherAngleX => 45f;

    public static SensorArmDirectionEnum GetArmDirection(bool witMotion, ScreenOrientation orientation, Vector3 gravity)
    {
        if (witMotion)
        {
            var rotDegreeX = Mathf.Asin(gravity.x) * Mathf.Rad2Deg;

            if (rotDegreeX <= lowerAngleX)
            {
                return SensorArmDirectionEnum.Down;
            }
            else if (rotDegreeX > higherAngleX)
            {
                return SensorArmDirectionEnum.Up;
            }
        }
        else
        {
            var rotGravity = gravity * CoodinateTransformer.LandscapeRotator(orientation);
            var rotDegreeX = Mathf.Asin(rotGravity.x) * Mathf.Rad2Deg;

            if (rotDegreeX <= lowerAngleX)
            {
                return SensorArmDirectionEnum.Down;
            }
            else if (rotDegreeX > higherAngleX)
            {
                return SensorArmDirectionEnum.Up;
            }
        }
        return SensorArmDirectionEnum.None;
    }
}
