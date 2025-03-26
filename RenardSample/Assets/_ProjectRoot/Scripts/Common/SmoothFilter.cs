using System;
using UnityEngine;

namespace SignageHADO
{
    public static class SmoothFilter
    {
        public static float ApplyFloat(float input, float before, float threshold, bool isDebugLog = false)
        {
            try
            {
                if (Math.Abs(input - before) <= threshold)
                    return before;
            }
            catch (Exception ex)
            {
                if (isDebugLog)
                    Debug.Log($"SmoothFilter::ApplyFloat - {ex.Message}");
            }
            return input;
        }

        public static bool ApplyFloatList(float[] list, float threshold, out float[] result, bool isDebugLog = false)
        {
            try
            {
                if (list == null || list.Length <= 0)
                    throw new Exception("not length list.");

                var previousValue = list[0];
                result = new float[list.Length];
                result[0] = previousValue;

                for (int i = 1; i < list.Length; i++)
                {
                    if (Math.Abs(list[i] - previousValue) > threshold)
                    {
                        previousValue = list[i];
                    }
                    result[i] = previousValue;
                }

                return true;
            }
            catch (Exception ex)
            {
                if (isDebugLog)
                    Debug.Log($"SmoothFilter::ApplyFloatList - {ex.Message}");

                result = list;
            }
            return false;
        }

        public static Vector3 ApplyVector3(Vector3 input, Vector3 before, Vector3 threshold, bool isDebugLog = false)
        {
            var result = input;

            try
            {
                if (Math.Abs(input.x - before.x) <= threshold.x)
                    result.x = before.x;

                if (Math.Abs(input.y - before.y) <= threshold.y)
                    result.y = before.y;

                if (Math.Abs(input.z - before.z) <= threshold.z)
                    result.z = before.z;
            }
            catch (Exception ex)
            {
                if (isDebugLog)
                    Debug.Log($"SmoothFilter::ApplyVector3 - {ex.Message}");

                result = input;
            }
            return result;
        }

        public static bool ApplyVector3List(Vector3[] list, Vector3 threshold, out Vector3[] result, bool isDebugLog = false)
        {
            try
            {
                if (list == null || list.Length <= 0)
                    throw new Exception("not length list.");

                var previousValue = list[0];
                result = new Vector3[list.Length];
                result[0] = previousValue;

                for (int i = 1; i < list.Length; i++)
                {
                    if (Math.Abs(list[i].x - previousValue.x) > threshold.x)
                    {
                        previousValue.x = list[i].x;
                    }

                    if (Math.Abs(list[i].y - previousValue.y) > threshold.y)
                    {
                        previousValue.y = list[i].y;
                    }

                    if (Math.Abs(list[i].y - previousValue.y) > threshold.y)
                    {
                        previousValue.y = list[i].y;
                    }

                    result[i] = previousValue;
                }

                return true;
            }
            catch (Exception ex)
            {
                if (isDebugLog)
                    Debug.Log($"SmoothFilter::ApplyFloatList - {ex.Message}");

                result = list;
            }
            return false;
        }
    }
}
