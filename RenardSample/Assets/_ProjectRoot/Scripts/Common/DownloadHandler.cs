using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SignageHADO
{
    using Renard;
    using Net;

    [Serializable]
    public class DownloadHandler : DownloadScript
    {
        protected class OrderType
        {
            public const int None = 0x00;
            public const int AssetBundle = 0x01;
        }

        [NonSerialized] private (byte[], DateTime) _resultData = default;
        [NonSerialized] private string _responseFileName = string.Empty;
        [NonSerialized] private byte[] _responseSendBytes = default;
        [NonSerialized] private DownloadResponseInfo _resultResponseInfo = null;
        [NonSerialized] private DownloadResponseError _resultResponseError = null;
        [NonSerialized] private DownloadRequestOrder _requestOrder = null;

        public string RequestCallMessage { get; protected set; } = string.Empty;

        #region Host

        protected override async UniTask OnSetupHostAsync(CancellationToken token)
        {
            SetupProgress = "download list create...";
            await UniTask.Delay(TimeSpan.FromMilliseconds(DownloadUtil.Interval.Process), cancellationToken: token);
            token.ThrowIfCancellationRequested();

            SetupProgress = "AssetBundle setup...";
            await UniTask.Delay(TimeSpan.FromMilliseconds(DownloadUtil.Interval.Process), cancellationToken: token);
            token.ThrowIfCancellationRequested();

            if (!await AssetBundleHandler.SetupAsync(token, resourcesPath, true))
            {
                SetupErrorMessages = "AssetBundle setup error.";
                throw new Exception("AssetBundle setup error.");
            }
        }

        protected override void OnReceivedRequestOrder(DownloadRequestOrder request)
        {
            if (request == null || string.IsNullOrEmpty(request.FromDeviceId) || string.IsNullOrEmpty(request.FromAddress))
                return;

            try
            {
                if (!CheckedStringAddress(request.FromAddress))
                    throw new Exception($"address parse error. from: {request.FromAddress}|{request.FromDeviceId}");

                var cancellationToken = new CancellationTokenSource();

                RequestCallMessage = $"Request: {request.FromAddress}|{request.FromDeviceId} {request.OrderType}";

                if (requestTokenPool.ContainsKey(request.FromDeviceId))
                {
                    requestTokenPool[request.FromDeviceId]?.Cancel();
                    requestTokenPool[request.FromDeviceId]?.Dispose();
                    requestTokenPool[request.FromDeviceId] = cancellationToken;
                }
                else
                {
                    requestTokenPool.Add(request.FromDeviceId, cancellationToken);
                }

                if (request.OrderType == OrderType.None)
                {
                    OnSendResponseErrorAsync(cancellationToken.Token, request.FromDeviceId, request.FromAddress, DownloadUtil.ErrorCode.RequestCode, $"request orderType error. {request.OrderType}").Forget();
                    throw new Exception($"request error. {request.OrderType}|{request.OrderValue}|{request.OrderFile}");
                }

                if (request.OrderType == OrderType.AssetBundle)
                {
                    OnResponseAssetBundleAsync(cancellationToken.Token, request.FromDeviceId, request.FromAddress, request.Platform, request.OrderFile, (request.OrderCode == DownloadUtil.OrderCode.DownloadData)).Forget();
                }
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnReceivedRequestOrder", $"{ex.Message}");
            }
            finally
            {
                RequestCallMessage = string.Empty;
            }
        }

        private async UniTask OnSendResponseErrorAsync(CancellationToken token, string deviceId, string address, int errorCode, string message)
        {
            _resultResponseError = new DownloadResponseError(deviceId, errorCode, message);
            _responseSendBytes = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(_resultResponseError));

            await OnSendResponseAsync(token, address, _responseSendBytes);
        }

        private async UniTask OnResponseAssetBundleAsync(CancellationToken token, string deviceId, string address, RuntimePlatform platform, string fileName, bool dataUpload)
        {
            _resultResponseInfo = null;
            _resultData = default;

            if (string.IsNullOrEmpty(fileName))
            {
                _resultData = AssetBundleHandler.GetHashListBinary(platform);
                _responseFileName = $"{AssetBundleManager.HashFileName}.{AssetBundleManager.HashFileExtension}";
            }
            else
            {
                _resultData = await AssetBundleHandler.GetAssetDataBinary(token, platform, fileName);
                _responseFileName = fileName;
            }

            if (_resultData.Item1 == null || _resultData.Item1.Length <= 0)
            {
                await OnSendResponseErrorAsync(token, deviceId, address, DownloadUtil.ErrorCode.NotData, $"not request data. AssetBundle[{platform}]| {_responseFileName}");
                return;
            }

            if (dataUpload)
            {
                _responseSendBytes = _resultData.Item1 != null ? _resultData.Item1 : null;
            }
            else
            {
                _resultResponseInfo = new DownloadResponseInfo(deviceId, _responseFileName, (_resultData.Item1 != null ? _resultData.Item1.Length : 0), _resultData.Item2);
                _responseSendBytes = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(_resultResponseInfo));
            }

            await OnSendResponseAsync(token, address, _responseSendBytes);
        }

        #endregion

        #region Client

        protected override async UniTask OnSetupClientAsync(CancellationToken token)
        {
            SetupProgress = "LocalDB setup...";
            await UniTask.Delay(TimeSpan.FromMilliseconds(DownloadUtil.Interval.Process), cancellationToken: token);
            token.ThrowIfCancellationRequested();

            SetupProgress = "AssetBundle setup...";
            await UniTask.Delay(TimeSpan.FromMilliseconds(DownloadUtil.Interval.Process), cancellationToken: token);
            token.ThrowIfCancellationRequested();

            if (!await AssetBundleHandler.SetupAsync(token))
            {
                SetupErrorMessages = "AssetBundle setup error.";
                Log(DebugerLogType.Info, "OnSetupResourcesAsync", "AssetBundle setup error.");
            }
        }

        protected override async UniTask<bool> OnLoadClientAsync(CancellationToken token)
        {
            _requestOrder = new DownloadRequestOrder(myDeviceId, ipAddress, platform, DownloadAddress);

            if (!await OnDownloadAssetBundleHashListAsync(token))
                return false;

            if (!await OnSetupAssetBundleAsync(token))
                return false;

            return true;
        }

        protected override async UniTask<bool> OnDownloadClientAsync(CancellationToken token)
        {
            var failed = false;

            try
            {
                SetupProgress = "AssetBundle download...";
                await UniTask.Delay(TimeSpan.FromMilliseconds(DownloadUtil.Interval.Process), cancellationToken: token);

                if (!await OnDownloadAssetBundleAsync(token))
                {
                    SetupErrorMessages = "AssetBundle download error.";
                    Log(DebugerLogType.Info, "OnDownloadClientAsync", "AssetBundle download error.");
                    failed = true;
                }

                SetupProgress = "resource reload...";

                if (!await OnReloadClientAsync(token))
                    throw new Exception("download after setup error.");

                return !failed;
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnDownloadClientAsync", $"{ex.Message}");
                return false;
            }
            finally
            {
                DownloadProgress?.Stop();
            }
        }

        private async UniTask<bool> OnReloadClientAsync(CancellationToken token)
        {
            var failed = false;

            try
            {
                if (!await OnSetupAssetBundleAsync(token))
                    failed = true;

                return !failed;
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnReloadClientAsync", $"{ex.Message}");
                return false;
            }
        }

        //-- AssetBundle

        private async UniTask<bool> OnSetupAssetBundleAsync(CancellationToken token)
        {
            try
            {
                await AssetBundleHandler.LoadHashListAsync(token);
                token.ThrowIfCancellationRequested();

                await AssetBundleHandler.FixedDifference(token);
                token.ThrowIfCancellationRequested();

                return true;
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnSetupAssetBundleAsync", $"{ex.Message}");
                SetupErrorMessages = $"OnSetupAssetBundleAsync: {ex.Message}";
                return false;
            }
        }

        private async UniTask<bool> OnDownloadAssetBundleHashListAsync(CancellationToken token)
        {
            try
            {
                // HashList取得
                _requestOrder.SetOrder(OrderType.AssetBundle);

                _resultResponseInfo = await OnSendRequestInfoAsync(token, _requestOrder);

                if (_resultResponseInfo == null || string.IsNullOrEmpty(_resultResponseInfo.DataName) || _resultResponseInfo.DataLength <= 0)
                    throw new Exception($"not download info. {(_resultResponseInfo != null ? _resultResponseInfo.DataName : "null")}: {(_resultResponseInfo != null ? _resultResponseInfo.DataLength.ToString() : "---")}");

                if (!await OnSendRequestDataAsync(token, _requestOrder, _resultResponseInfo, AssetBundleHandler.AssetDirectoryPath))
                    throw new Exception($"not write download data. {AssetBundleHandler.AssetDirectoryPath}/{_resultResponseInfo.DataName}");

                return true;
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnDownloadAssetBundleHashListAsync", $"{ex.Message}");
                SetupErrorMessages = $"OnDownloadAssetBundleHashListAsync: {ex.Message}";
                return false;
            }
        }

        private async UniTask<bool> OnDownloadAssetBundleAsync(CancellationToken token)
        {
            // 差分なし
            if (!AssetBundleHandler.IsDiffData) return true;

            var count = 0;
            var failed = false;

            DownloadProgress?.Start("AssetBundle download...");
            DownloadProgress?.SetCount("AssetBundle", AssetBundleHandler.DiffDataList.Count);

            try
            {
                foreach (var item in AssetBundleHandler.DiffDataList)
                {
                    token.ThrowIfCancellationRequested();

                    try
                    {
                        _requestOrder.SetOrder(OrderType.AssetBundle, $"{item}");

                        _resultResponseInfo = await OnSendRequestInfoAsync(token, _requestOrder);

                        if (_resultResponseInfo == null || string.IsNullOrEmpty(_resultResponseInfo.DataName) || _resultResponseInfo.DataLength <= 0)
                            throw new Exception($"not download info. {(_resultResponseInfo != null ? _resultResponseInfo.DataName : "null")}: {(_resultResponseInfo != null ? _resultResponseInfo.DataLength.ToString() : "---")}");

                        DownloadProgress?.SetData(_resultResponseInfo.DataName, _resultResponseInfo.DataLength);

                        if (!await OnSendRequestDataAsync(token, _requestOrder, _resultResponseInfo, AssetBundleHandler.AssetDirectoryPath))
                            throw new Exception($"not write download data. {AssetBundleHandler.AssetDirectoryPath}/{_resultResponseInfo.DataName}");
                    }
                    catch (Exception ex)
                    {
                        Log(DebugerLogType.Info, "OnDownloadAssetBundleAsync(Data DL)", $"{ex.Message}");
                        failed = true;
                    }

                    count++;

                    DownloadProgress?.UpdateCount(count);
                }

                DownloadProgress?.Stop();

                await AssetBundleHandler.LoadAsync(token);
                token.ThrowIfCancellationRequested();
                return !failed;
            }
            catch (Exception ex)
            {
                DownloadProgress?.Stop();

                Log(DebugerLogType.Info, "OnDownloadAssetBundleAsync", $"{ex.Message}");
                return false;
            }
        }

        #endregion
    }
}