using System;
using UnityEngine;

namespace SignageHADO
{
    public static class FloatExtensions
    {
        public static float Slerp(float from, float to, float slerp)
        {
            try
            {
                if (slerp >= 0)
                    return Mathf.Lerp(from, to, Mathf.Sin(Mathf.Lerp(0, Mathf.PI / 2, Mathf.Clamp01(slerp))));
            }
            catch
            {
                // 何もしない
            }
            return to;
        }
    }
}
