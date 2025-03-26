using System;
using UnityEngine;

namespace SignageHADO
{
    [Serializable]
    public class DownloadProgressInfo
    {
        public bool IsActive { get; private set; } = false;

        public string Title { get; private set; } = string.Empty;
        public string SubTitle { get; private set; } = string.Empty;
        public float Value { get; private set; } = 0f;
        public int MaxCount { get; private set; } = 0;
        public int CurrentCount { get; private set; } = 0;

        public string DataTitle { get; private set; } = string.Empty;
        public float ValueData { get; private set; } = 0f;
        public long MaxDataLength { get; private set; } = 0;
        public long CurrentDataLength { get; private set; } = 0;

        public void Reset()
        {
            SubTitle = string.Empty;
            MaxCount = 0;
            CurrentCount = 0;
            Value = 0f;

            DataTitle = string.Empty;
            MaxDataLength = 0;
            CurrentDataLength = 0;
            ValueData = 0f;
        }

        public void Start(string title)
        {
            Title = title;
            IsActive = true;
            Reset();
        }

        public void SetCount(string subTitle, int maxCount)
        {
            SubTitle = subTitle;
            MaxCount = maxCount;
            CurrentCount = 0;
            Value = 0f;
        }

        public void SetData(string dataTitle, long maxLength)
        {
            DataTitle = dataTitle;
            MaxDataLength = maxLength;
            CurrentDataLength = 0;
            ValueData = 0f;
        }

        public void UpdateCount(int count)
        {
            CurrentCount = count;

            try
            {
                Value = MaxCount <= 0 ? 0f : (float)CurrentCount / (float)MaxCount;
            }
            catch
            {
                // 何もしない
            }
        }

        public void UpdateData(long currentLength)
        {
            CurrentDataLength = currentLength;

            try
            {
                ValueData = MaxDataLength <= 0 ? 0f : (float)((double)CurrentDataLength / (double)MaxDataLength);
            }
            catch
            {
                // 何もしない
            }
        }

        public void Stop()
        {
            IsActive = false;
        }
    }
}

namespace SignageHADO.Net
{
    public static class DownloadUtil
    {
        /// <summary>※単位はミリ秒</summary>
        public static class Interval
        {
            /// <summary></summary>
            public const int Setup = 100;
            /// <summary></summary>
            public const int Connection = 100;
            /// <summary</summary>
            public const int ConnectionStartWait = 200;
            /// <summary</summary>
            public const int Process = 150;
            /// <summary</summary>
            public const int Request = 100;
        }

        /// <summary>※単位はミリ秒</summary>
        public static class Timeout
        {
            /// <summary></summary>
            public const int Setup = 500;
            /// <summary></summary>
            public const int Connection = 500;
            /// <summary</summary>
            public const int Request = 500;

            /// <summary</summary>
            public const int Accept = 3 * 1000;
            /// <summary</summary>
            public const int Read = 30 * 1000;
            /// <summary</summary>
            public const int Write = 10 * 1000;
            /// <summary</summary>
            public const int WriteFileReadLock = 1 * 1000;
        }

        /// <summary></summary>
        public const int SetupRetry = 3;
        /// <summary></summary>
        public const int ConnectionRetry = 5;
        /// <summary></summary>
        public const int RequestRetry = 5;
        /// <summary>※単位はバイト</summary>
        public const int ReceiveResponseBuffer = 16 * 1024;

        /// <summary></summary>
        public static class OrderCode
        {
            public const int None = 0x00;

            /// <summary>情報取得</summary>
            public const int DownloadInfo = 0x01;
            /// <summary>データ取得</summary>
            public const int DownloadData = 0x02;
        }

        /// <summary></summary>
        public static class ErrorCode
        {
            public const int None = 0x00;

            /// <summary>コード不正</summary>
            public const int RequestCode = 0x01;
            /// <summary>データなし</summary>
            public const int NotData = 0x02;
        }

        // 上り最低5Mbps以上が出る前提
        private const float bpsSendMin = 3f * 1024f * 1024f;

        // 下り最低5Mbps以上が出る前提
        private const float bpsReceiveMin = 5f * 1024f * 1024f;

        // 伝送効率
        private const float transmissionEfficiency = 0.6f;

        // 最短タイムアウト時間[ms]
        private const int minTimeoutMillisecond = 100;

        private static float OnGetSendTimeMillisecond(long size, float speed, float efficiency)
            => size > 0 ? (size / speed * efficiency) * 1000f : 0f;

        private static int OnGetTimeOutMillisecond(float time)
            => time > minTimeoutMillisecond ? (int)time : (int)minTimeoutMillisecond;

        /// <summary>読込みタイムアウト時間[ms]</summary>
        public static int GetReadTimeOutMillisecond(long size)
            => OnGetTimeOutMillisecond(OnGetSendTimeMillisecond(size * 8, bpsSendMin, transmissionEfficiency));

        /// <summary>書込みタイムアウト時間[ms]</summary>
        public static int GetWriteTimeOutMillisecond(long size)
            => OnGetTimeOutMillisecond(OnGetSendTimeMillisecond(size * 8, bpsReceiveMin, transmissionEfficiency));
    }

    [Serializable]
    public class DownloadRequestOrder
    {
        public string FromDeviceId = string.Empty;
        public string FromAddress = string.Empty;

        [SerializeField] private int _platform = (int)RuntimePlatform.WindowsPlayer;
        public RuntimePlatform Platform
        {
            get => (RuntimePlatform)_platform;
            set => _platform = (int)value;
        }

        public int OrderCode = DownloadUtil.OrderCode.None;
        public int OrderType = 0;

        public string OrderFile = string.Empty;
        public int OrderValue = 0;
        public int OrderValueSub = 0;

        public string ToAddress = string.Empty;

        public DownloadRequestOrder(string fromDeviceId, string address, RuntimePlatform platform, string toAddress)
        {
            FromDeviceId = fromDeviceId;
            FromAddress = address;
            Platform = platform;
            ToAddress = toAddress;
        }

        public void SetOrder(int orderType)
            => SetOrder(orderType, string.Empty, 0);

        public void SetOrder(int orderType, int orderValue, int orderValueSub = 0)
            => SetOrder(orderType, string.Empty, orderValue);

        public void SetOrder(int orderType, string orderFile)
            => SetOrder(orderType, orderFile, 0);

        public void SetOrder(int orderType, string orderFile, int orderValue, int orderValueSub = 0)
        {
            OrderType = orderType;
            OrderFile = orderFile;
            OrderValue = orderValue;
            OrderValueSub = orderValueSub;
        }
    }

    [Serializable]
    public class DownloadResponseTemplate
    {
        public string ToDeviceId = string.Empty;
        public int ErrorCode = DownloadUtil.ErrorCode.None;
    }

    [Serializable]
    public class DownloadResponseError : DownloadResponseTemplate
    {
        public string Message = string.Empty;

        public DownloadResponseError(string deviceId, int errorCode, string message)
        {
            ToDeviceId = deviceId;

            ErrorCode = errorCode;
            Message = message;
        }
    }

    [Serializable]
    public class DownloadResponseInfo : DownloadResponseTemplate
    {
        public string DataName = string.Empty;
        public long DataLength = 0;

        [SerializeField] private long _dataTimestamp = DateTime.UtcNow.ToBinary();
        public DateTime DataTimestamp
        {
            get => DateTime.FromBinary(_dataTimestamp);
            set => _dataTimestamp = value.ToBinary();
        }

        public DownloadResponseInfo(string toDeviceId, string dataName, long dataLength, DateTime dataTimestamp)
        {
            ToDeviceId = toDeviceId;

            DataName = dataName;
            DataLength = dataLength;
            DataTimestamp = dataTimestamp;
        }
    }
}
