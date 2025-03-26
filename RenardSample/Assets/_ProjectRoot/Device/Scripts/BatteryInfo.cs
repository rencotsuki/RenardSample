using System.Collections;
using UnityEngine;
using System.Runtime.InteropServices;

namespace SignageHADO
{
    /// <summary>バッテリー状態</summary>
    public enum BatteryStatusEnum : int
    {
        /// <summary>バッテリー無し</summary>
        None = 0,
        /// <summary>バッテリー使用中</summary>
        Unplug = 1,
        /// <summary>チャージ中</summary>
        Charge = 2,
    }

    public class BatteryInfo
    {
        /// <summary>バッテリー低下水準</summary>
        public static float LowLevel => 0.4f;

#if UNITY_ANDROID && !UNITY_EDITOR

        private static AndroidJavaClass _overseer = null;
        protected static AndroidJavaClass overseer => _overseer ?? (_overseer = new AndroidJavaClass("com.volpe.DeviceBatteryInfo"));

#elif UNITY_IOS && !UNITY_EDITOR

        [DllImport("__Internal")] private static extern float BatteryLevelNative();
        [DllImport("__Internal")] private static extern int BatteryStatusNative();

#endif

        /// <summary>バッテリー容量[0.0～1.0] ※0未満はバッテリー未搭載</summary>
        public static float Level
        {
            get
            {
#if UNITY_ANDROID && !UNITY_EDITOR

                if (overseer != null)
                {
                    return (float)overseer.CallStatic<int>("Level") / (float)overseer.CallStatic<int>("MaxLevel");
                }

#elif UNITY_IOS && !UNITY_EDITOR

                return BatteryLevelNative();

#endif
                return SystemInfo.batteryLevel;
            }
        }

        private static int _statusValue
        {
            get
            {
#if UNITY_ANDROID && !UNITY_EDITOR

                if (overseer != null)
                    return overseer.CallStatic<int>("Status");

#elif UNITY_IOS && !UNITY_EDITOR

                return BatteryStatusNative();

#endif
                return -1;
            }
        }

        /// <summary>バッテリー状態</summary>
        public static BatteryStatusEnum Status
        {
            get
            {
#if UNITY_ANDROID && !UNITY_EDITOR

                if (_statusValue == 1 || _statusValue == 2)
                    return BatteryStatusEnum.Charge;
                
                if (_statusValue != -1)
                    return BatteryStatusEnum.Unplug;

#elif UNITY_IOS && !UNITY_EDITOR
                
                if (_statusValue > 0)
                    return BatteryStatusEnum.Charge;
                
                if (_statusValue != -1)
                    return BatteryStatusEnum.Unplug;

#else
                if (SystemInfo.batteryStatus == UnityEngine.BatteryStatus.Charging)
                    return BatteryStatusEnum.Charge;

                if (SystemInfo.batteryStatus != UnityEngine.BatteryStatus.Unknown)
                    return BatteryStatusEnum.Unplug;
#endif
                return BatteryStatusEnum.None;
            }
        }
    }
}
