using System.Collections;
using UnityEngine;
using System.Runtime.InteropServices;

namespace SignageHADO
{
    /// <summary>発熱レベル</summary>
    public enum ThermalStateEnum : int
    {
        /// <summary>熱状態は正常範囲内</summary>
        Nominal,

        /// <summary>熱状態がわずかに上昇</summary>
        Fair,

        /// <summary>熱状態が高い</summary>
        Serious,

        /// <summary>熱状態が非常に高くパフォーマンス低下</summary>
        Critical,
    }

    public class ProcessInfo
    {
#if UNITY_IOS && !UNITY_EDITOR

        [DllImport("__Internal")] private static extern int ThermalStateNative();

#endif

        private static int _statusValue
        {
            get
            {
#if UNITY_IOS && !UNITY_EDITOR

                return ThermalStateNative();

#endif
                return -1;
            }
        }

        /// <summary>発熱状態</summary>
        public static ThermalStateEnum Status
        {
            get
            {
                if (_statusValue >= 0)
                    return (ThermalStateEnum)_statusValue;

                return ThermalStateEnum.Nominal;
            }
        }
    }
}
