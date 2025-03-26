using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace SignageHADO
{
    using Sqlite;

    public class TrackingLogDB : TemplateDB
    {
        public const string TemplateFileName = "TrackingLogs";

        public const string TrackingLogTableName = "TrackingLog";

        /// <summary>トラッキングログ</summary>
        public class TrackingLog : DataColumn
        {
            /// <summary>トラッキングNo</summary>
            public int No = 0;

            /// <summary>計測した時間</summary>
            public string TrackingTime = string.Empty;

            /// <summary>電池残量</summary>
            public float BatteryPower = 0f;

            /// <summary>カメラのピッチ角</summary>
            public float CameraDevicePitch = 0f;

            /// <summary>カメラのヨー角</summary>
            public float CameraDeviceYaw = 0f;

            /// <summary>カメラのロール角</summary>
            public float CameraDeviceRoll = 0f;

            /// <summary>人の検知数</summary>
            public int PersonCount = 0;

            /// <summary>顔認識状態</summary>
            public bool HeadTracking = false;

            /// <summary>左手認識状態</summary>
            public bool LeftHandTracking = false;

            /// <summary>右手認識状態</summary>
            public bool RightHandTracking = false;

            public static TrackingLog New(DataRow dr = null)
            {
                var data = new TrackingLog();
                data.Set(dr);
                return data;
            }

            protected override void OnSet(DataRow dr)
            {
                No = dr.Keys.Contains("No") ? (int)dr["No"] : 0;

                TrackingTime = dr.Keys.Contains("TrackingTime") ? (string)dr["TrackingTime"] : string.Empty;

                var doubleBatteryPower = dr.Keys.Contains("BatteryPower") ? (double)dr["BatteryPower"] : 0d;
                BatteryPower = (float)doubleBatteryPower;

                var doubleCameraDevicePitch = dr.Keys.Contains("CameraDevicePitch") ? (double)dr["CameraDevicePitch"] : 0d;
                CameraDevicePitch = (float)doubleCameraDevicePitch;

                var doubleCameraDeviceYaw = dr.Keys.Contains("CameraDeviceYaw") ? (double)dr["CameraDeviceYaw"] : 0d;
                CameraDeviceYaw = (float)doubleCameraDeviceYaw;

                var doubleCameraDeviceRoll = dr.Keys.Contains("CameraDeviceRoll") ? (double)dr["CameraDeviceRoll"] : 0d;
                CameraDeviceRoll = (float)doubleCameraDeviceRoll;

                PersonCount = dr.Keys.Contains("PersonCount") ? (int)dr["PersonCount"] : 0;

                HeadTracking = dr.Keys.Contains("HeadTracking") ? ((int)dr["HeadTracking"] == 1) : false;

                LeftHandTracking = dr.Keys.Contains("LeftHandTracking") ? ((int)dr["LeftHandTracking"] == 1) : false;

                RightHandTracking = dr.Keys.Contains("RightHandTracking") ? ((int)dr["RightHandTracking"] == 1) : false;
            }
        }

        #region API

        public static async UniTask<TrackingLog[]> GetTrackingLogData(CancellationToken cancellationToken, string directoryPath, string dbFileName)
        {
            return await OnExecuteQueryCoroutine<TrackingLog>(
                cancellationToken,
                directoryPath,
                dbFileName,
                CreateSelectQuery(TrackingLogTableName, null));
        }

        #endregion
    }
}
