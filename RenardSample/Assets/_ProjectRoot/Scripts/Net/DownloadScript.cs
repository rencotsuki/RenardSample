using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;

namespace SignageHADO.Net
{
    [Serializable]
    public class DownloadScript : MonoBehaviourCustom
    {
        protected class TcpListenerCustom : TcpListener
        {
            public TcpListenerCustom(IPEndPoint localEP) : base(localEP) { }
            public TcpListenerCustom(IPAddress localaddr, int port) : base(localaddr, port) { }

            public bool IsActive => Active;
        }

        public bool IsHost { get; private set; } = false;
        public bool IsSetup { get; private set; } = false;
        public bool IsRequestListening { get; private set; } = false;
        public bool IsDownloading { get; private set; } = false;

        public bool IsSettingUp { get; private set; } = false;
        public string SetupProgress { get; protected set; } = string.Empty;
        public DownloadProgressInfo DownloadProgress { get; protected set; } = new DownloadProgressInfo();
        public string SetupErrorMessages { get; protected set; } = string.Empty;

        protected string myDeviceId => SystemInfo.deviceUniqueIdentifier;

        protected RuntimePlatform platform
        {
            get
            {
#if UNITY_EDITOR
                if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android)
                    return RuntimePlatform.Android;

                if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS)
                    return RuntimePlatform.IPhonePlayer;

                if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.StandaloneOSX)
                    return RuntimePlatform.OSXPlayer;

                return RuntimePlatform.WindowsPlayer;
#else
                return Application.platform;
#endif
            }
        }

        public string DownloadAddress { get; protected set; } = string.Empty;
        public int DownloadPort { get; protected set; } = 0;
        protected string ipAddress { get; private set; } = string.Empty;
        protected int responsePort => DownloadPort + 1;

        protected string resourcesPath { get; private set; } = string.Empty;

        protected Dictionary<string, CancellationTokenSource> requestTokenPool = new Dictionary<string, CancellationTokenSource>();

        [HideInInspector] private IPEndPoint _ipEndPointRequest = null;
        [HideInInspector] private byte[] _resultResponseBytes = null;
        [HideInInspector] private DownloadResponseInfo _resultResponse = null;

        private IDisposable _onProgressObserve = null;
        private CancellationTokenSource _onRequestListenerToken = null;

        private void OnDestroy()
        {
            Exit();
        }

        protected T TryFromJson<T>(byte[] bytesData) where T : class
        {
            try
            {
                return JsonUtility.FromJson<T>(System.Text.Encoding.UTF8.GetString(bytesData));
            }
            catch
            {
                return null;
            }
        }

        protected bool CheckedStringAddress(string address)
        {
            if (!string.IsNullOrEmpty(address))
            {
                IPAddress parseAddress = null;
                if (IPAddress.TryParse(address, out parseAddress))
                    return true;
            }
            return false;
        }

        private async UniTask<bool> OnSetupResourcesAsync(CancellationToken token, string path)
        {
            resourcesPath = path;

            try
            {
                if (string.IsNullOrEmpty(resourcesPath))
                    throw new Exception($"not path. resourcesPath={resourcesPath}");

                if (IsHost)
                {
                    await OnSetupHostAsync(token);
                }
                else
                {
                    await OnSetupClientAsync(token);
                }

                await UniTask.Delay(TimeSpan.FromMilliseconds(DownloadUtil.Interval.Process), cancellationToken: token);

                return true;
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnSetupResourcesAsync", $"{ex.Message}");
                return false;
            }
        }

        /// <summary>強制終了</summary>
        public void Exit()
        {
            OnDisableHost();
            OnDisableClient();
        }

        #region Host

        private void OnDisableHost()
        {
            IsSetup = false;

            OnDisposeRequestListener();
            OnClearRequestPool();
        }

        /// <summary>初期設定：ホスト側</summary>
        public async UniTask<bool> SetupHostAsync(CancellationToken token, string address, int port, string resourcesPath)
        {
            IsHost = true;

            IsSettingUp = true;
            SetupProgress = string.Empty;
            SetupErrorMessages = string.Empty;

            OnDisableHost();

            try
            {
                SetupProgress = "download settings...";

                if (!NetConfigs.IsEffectiveAddress(address) || port <= 0)
                    throw new Exception($"address error. address={address}, port={port}");

                ipAddress = DownloadAddress = address;
                DownloadPort = port;

                SetupProgress = "resources setup...";

                if (!await OnSetupResourcesAsync(token, resourcesPath))
                    throw new Exception($"setup resources error.");

                OpenRequestListener();

                IsSetup = true;
            }
            catch (Exception ex)
            {
                IsSetup = false;

                Log(DebugerLogType.Info, "SetupHostAsync", $"{ex.Message}");
                SetupErrorMessages = $"error - {SetupProgress} | downloadAddress: {address}({port}),\n\rresourcesPath={resourcesPath}\n\r{ex.Message}";
            }
            finally
            {
                IsSettingUp = false;
                SetupProgress = string.Empty;
            }

            return IsSetup;
        }

        protected virtual async UniTask OnSetupHostAsync(CancellationToken token)
        {
            // オーバーライドして、ここに初期処理を書く
        }

        /// <summary>リクエストオープン【Server】</summary>
        public void OpenRequestListener()
        {
            OnDisposeRequestListener();
            _onRequestListenerToken = new CancellationTokenSource();
            OnRequestListenerAsync(_onRequestListenerToken.Token).Forget();
        }

        private void OnDisposeRequestListener()
        {
            _onRequestListenerToken?.Cancel();
            _onRequestListenerToken?.Dispose();
            _onRequestListenerToken = null;
        }

        private async UniTask OnRequestListenerAsync(CancellationToken token)
        {
            UdpClient udpClientConnection = null;

            try
            {
                var ipEndPointConnection = new IPEndPoint(IPAddress.Any, DownloadPort);
                ipEndPointConnection?.Create(new SocketAddress(AddressFamily.InterNetwork));

                var retryCount = DownloadUtil.SetupRetry;

                do
                {
                    try
                    {
                        udpClientConnection = new UdpClient(ipEndPointConnection);
                        break;
                    }
                    catch (Exception ex)
                    {
                        Log(DebugerLogType.Info, "OnRequestListenerAsync", $"{ex.Message}");
                        udpClientConnection = null;
                    }

                    retryCount--;

                    await UniTask.Delay(TimeSpan.FromMilliseconds(DownloadUtil.Interval.Setup), cancellationToken: token);
                    token.ThrowIfCancellationRequested();
                }
                while (retryCount > 0);

                if (udpClientConnection == null)
                    throw new Exception("setup UdpClient error.");

                UdpReceiveResult receiveResult = default;
                byte[] resultBytes = null;
                DownloadRequestOrder result = null;
                CancellationTokenSource linkedTokenSource = null;

                IsRequestListening = true;

                while (_onRequestListenerToken != null)
                {
                    token.ThrowIfCancellationRequested();

                    try
                    {
                        linkedTokenSource?.Dispose();
                        linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);

                        receiveResult = await udpClientConnection.ReceiveAsync()
                                                    .AsUniTask()
                                                    .AttachExternalCancellation(linkedTokenSource.Token);

                        if (receiveResult != null && receiveResult.RemoteEndPoint != null)
                        {
                            if (!IsSetup) continue;

                            resultBytes = receiveResult.Buffer;
                            if (resultBytes != null && resultBytes.Length > 0)
                            {
                                try
                                {
                                    result = TryFromJson<DownloadRequestOrder>(resultBytes);
                                    if (result == null || string.IsNullOrEmpty(result.FromDeviceId))
                                        throw new Exception($"request error. fromDeviceId={(result != null ? result.FromDeviceId : "---")}");

                                    if (result.ToAddress != ipAddress)
                                        throw new Exception($"request address error. {ipAddress}|{result.ToAddress}");

                                    OnReceivedRequestOrder(result);
                                }
                                catch (Exception ex)
                                {
                                    Log(DebugerLogType.Info, "OnRequestListenerAsync", $"{ex.Message}");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log(DebugerLogType.Info, "OnRequestListenerAsync", $"{ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnRequestListenerAsync", $"{ex.Message}");

                udpClientConnection?.Dispose();
            }
            finally
            {
                udpClientConnection?.Close();
                IsRequestListening = false;
            }
        }

        private void OnClearRequestPool()
        {
            if (requestTokenPool == null || requestTokenPool.Count <= 0)
                return;

            for (int i = 0; i < requestTokenPool.Count; i++)
            {
                requestTokenPool.ElementAt(i).Value?.Cancel();
                requestTokenPool.ElementAt(i).Value?.Dispose();
            }

            requestTokenPool.Clear();
        }

        protected async UniTask OnSendResponseAsync(CancellationToken token, string address, byte[] sendBytes)
        {
            TcpClient tcpClient = null;
            NetworkStream stream = null;

            try
            {
                await UniTask.Delay(TimeSpan.FromMilliseconds(DownloadUtil.Interval.ConnectionStartWait), cancellationToken: token);

                var connectionTimeoutToken = new CancellationTokenSource();
                connectionTimeoutToken.CancelAfterSlim(TimeSpan.FromMilliseconds(DownloadUtil.Timeout.Connection));
                var tcpClientLinkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, connectionTimeoutToken.Token);

                var retryCount = DownloadUtil.ConnectionRetry;

                do
                {
                    try
                    {
                        tcpClient = new TcpClient();
                        await tcpClient.ConnectAsync(IPAddress.Parse(address), responsePort)
                                    .AsUniTask()
                                    .AttachExternalCancellation(tcpClientLinkedTokenSource.Token);
                        break;
                    }
                    catch (Exception ex)
                    {
                        Log(DebugerLogType.Info, "OnSendResponseAsync", $"{ex.Message}");
                        tcpClient = null;
                    }

                    retryCount--;

                    await UniTask.Delay(TimeSpan.FromMilliseconds(DownloadUtil.Interval.Connection), cancellationToken: token);
                    token.ThrowIfCancellationRequested();
                }
                while (retryCount > 0);

                token.ThrowIfCancellationRequested();

                stream = tcpClient?.GetStream();
                if (stream == null)
                    throw new Exception("stream not found.");

                var writeTimeout = DownloadUtil.GetWriteTimeOutMillisecond(sendBytes.Length);

                var writeTimeoutToken = new CancellationTokenSource();
                writeTimeoutToken.CancelAfterSlim(TimeSpan.FromMilliseconds(writeTimeout <= 0 ? DownloadUtil.Timeout.Write : writeTimeout));
                var streamLinkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, writeTimeoutToken.Token);

                await stream.WriteAsync(sendBytes, 0, sendBytes.Length, streamLinkedTokenSource.Token);

                token.ThrowIfCancellationRequested();
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnSendResponseAsync", $"{address}({responsePort}) {ex.Message}");

                stream?.Dispose();
                tcpClient?.Dispose();
            }
            finally
            {
                stream?.Close();
                tcpClient?.Close();
            }
        }

        protected virtual void OnReceivedRequestOrder(DownloadRequestOrder request)
        {
            if (request == null || string.IsNullOrEmpty(request.FromDeviceId) || string.IsNullOrEmpty(request.ToAddress))
                return;

            try
            {
                if (!CheckedStringAddress(request.ToAddress))
                    throw new Exception($"address parse error. toAddress: {request.ToAddress}");

                /**
                var cancellationToken = new CancellationTokenSource();

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

                // オーバーライドして、ここに返す処理を書く

                **/
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnReceivedRequestOrder", $"{request.OrderCode}|{request.FromDeviceId}: {ex.Message}");
            }
        }

        #endregion

        #region Client

        protected void OnDisableClient()
        {
            IsSetup = false;
        }

        /// <summary>初期設定：クライアント側</summary>
        public async UniTask<bool> SetupClientAsync(CancellationToken token, string address, string serverAddress, int downloadPort, string resourcesPath)
        {
            IsHost = false;

            IsSettingUp = true;
            SetupProgress = string.Empty;
            SetupErrorMessages = string.Empty;

            OnDisableClient();

            try
            {
                SetupProgress = "download settings...";

                if (!CheckedStringAddress(address) || !CheckedStringAddress(serverAddress) || downloadPort <= 0)
                    throw new Exception($"address error. address={address}, downloadAddress={serverAddress}({downloadPort})");

                ipAddress = address;
                DownloadAddress = serverAddress;
                DownloadPort = downloadPort;

                _ipEndPointRequest = new IPEndPoint(IPAddress.Parse(DownloadAddress), DownloadPort);
                _ipEndPointRequest?.Create(new SocketAddress(AddressFamily.InterNetwork));

                SetupProgress = "resources setup...";

                if (!await OnSetupResourcesAsync(token, resourcesPath))
                    throw new Exception($"setup resources error.");

                token.ThrowIfCancellationRequested();

                SetupProgress = "local resources load...";
                await UniTask.Delay(TimeSpan.FromMilliseconds(DownloadUtil.Interval.Process), cancellationToken: token);
                token.ThrowIfCancellationRequested();

                if (!await OnLoadClientAsync(token))
                    throw new Exception("setup load error.");

                token.ThrowIfCancellationRequested();

                SetupProgress = "download resources check...";
                await UniTask.Delay(TimeSpan.FromMilliseconds(DownloadUtil.Interval.Process), cancellationToken: token);
                token.ThrowIfCancellationRequested();

                if (!await OnDownloadClientAsync(token))
                    throw new Exception("setup load error.");

                token.ThrowIfCancellationRequested();

                IsSetup = true;
            }
            catch (Exception ex)
            {
                IsSetup = false;

                Log(DebugerLogType.Info, "SetupClientAsync", $"{ex.Message}");
                SetupErrorMessages = $"error - {SetupProgress} | address: {address},  downloadAddress: {serverAddress}({downloadPort}),\n\rresourcesPath={resourcesPath}\n\r{ex.Message}";
            }
            finally
            {
                IsSettingUp = false;
                SetupProgress = string.Empty;
            }

            return IsSetup;
        }

        protected virtual async UniTask OnSetupClientAsync(CancellationToken token)
        {
            // オーバーライドして、ここに初期処理を書く
        }

        protected virtual async UniTask<bool> OnLoadClientAsync(CancellationToken token)
        {
            // オーバーライドして、ここにロード処理を書く
            return true;
        }

        protected virtual async UniTask<bool> OnDownloadClientAsync(CancellationToken token)
        {
            // オーバーライドして、ここにダウンロード処理を書く
            return true;
        }

        private async UniTask<bool> OnSendRequestAsync(CancellationToken token, IPEndPoint ipEndPoint, byte[] sendRequest)
        {
            UdpClient udpClientRequest = null;

            try
            {
                if (sendRequest == null || sendRequest.Length <= 0)
                    throw new Exception("send request dataSize zero.");

                var timeoutToken = new CancellationTokenSource();
                timeoutToken.CancelAfterSlim(TimeSpan.FromMilliseconds(DownloadUtil.Timeout.Request));
                var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(token, timeoutToken.Token);

                udpClientRequest = new UdpClient();

                await udpClientRequest.SendAsync(sendRequest, sendRequest.Length, ipEndPoint)
                        .AsUniTask()
                        .AttachExternalCancellation(linkedToken.Token);

                token.ThrowIfCancellationRequested();

                return true;
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnSendRequestAsync", $"{ex.Message}");

                udpClientRequest?.Dispose();
                return false;
            }
            finally
            {
                udpClientRequest?.Close();
            }
        }

        private async UniTask<byte[]> OnReceivedResponseAsync(CancellationToken token, string ipAddress, int responsePort)
        {
            TcpListenerCustom tcpListener = null;
            TcpClient receiveResult = null;
            NetworkStream streamResponse = null;

            try
            {
                tcpListener = new TcpListenerCustom(new IPEndPoint(IPAddress.Parse(ipAddress), responsePort));
                if (tcpListener == null)
                    throw new Exception($"create tcpListener error. {ipAddress}({responsePort})");

                var waitTimeoutToken = new CancellationTokenSource();
                waitTimeoutToken.CancelAfterSlim(TimeSpan.FromMilliseconds(DownloadUtil.Timeout.Setup));
                var waitLinkedToken = CancellationTokenSource.CreateLinkedTokenSource(token, waitTimeoutToken.Token);

                tcpListener.Start();

                await UniTask.WaitWhile(() => !tcpListener.IsActive, PlayerLoopTiming.Update, waitLinkedToken.Token);

                token.ThrowIfCancellationRequested();

                if (!tcpListener.IsActive)
                    throw new Exception("start tcpListener error.");

                var acceptTimeoutToken = new CancellationTokenSource();
                acceptTimeoutToken.CancelAfterSlim(TimeSpan.FromMilliseconds(DownloadUtil.Timeout.Accept));
                var acceptLinkedToken = CancellationTokenSource.CreateLinkedTokenSource(token, acceptTimeoutToken.Token);

                receiveResult = await tcpListener.AcceptTcpClientAsync()
                                                .AsUniTask()
                                                .AttachExternalCancellation(acceptLinkedToken.Token);

                token.ThrowIfCancellationRequested();

                if (receiveResult != null)
                {
                    if (receiveResult.Client != null && receiveResult.Client.LingerState != null)
                    {
                        receiveResult.Client.LingerState.Enabled = false;
                        receiveResult.Client.LingerState.LingerTime = 0;
                    }

                    streamResponse = receiveResult.GetStream();
                }

                if (streamResponse == null)
                    throw new Exception("stream not found.");

                var resultBytes = new byte[DownloadUtil.ReceiveResponseBuffer];

                var readTimeoutToken = new CancellationTokenSource();
                readTimeoutToken.CancelAfterSlim(TimeSpan.FromMilliseconds(DownloadUtil.Timeout.Read));
                var readLinkedToken = CancellationTokenSource.CreateLinkedTokenSource(token, readTimeoutToken.Token);

                await streamResponse.ReadAsync(resultBytes, 0, resultBytes.Length, readLinkedToken.Token);

                token.ThrowIfCancellationRequested();

                if (resultBytes == null || resultBytes.Length <= 0)
                    throw new Exception("response dataSize zero.");

                return resultBytes;
            }
            catch (Exception ex)
            {
                //Log(DebugerLogType.Info, "OnReceivedResponseAsync", $"{ex.Message}");

                streamResponse?.Dispose();
                receiveResult?.Dispose();
                return null;
            }
            finally
            {
                streamResponse?.Close();
                receiveResult?.Close();
                tcpListener?.Stop();
            }
        }

        private async UniTask<bool> OnReceivedFileDataAsync(CancellationToken token, string ipAddress, int responsePort, DownloadResponseInfo info, string outputPath)
        {
            TcpListenerCustom tcpListener = null;
            TcpClient receiveResult = null;
            NetworkStream streamResponse = null;
            MemoryStream memoryResponse = null;

            try
            {
                tcpListener = new TcpListenerCustom(new IPEndPoint(IPAddress.Parse(ipAddress), responsePort));
                if (tcpListener == null)
                    throw new Exception($"create tcpListener error. {ipAddress}({responsePort})");

                var waitTimeoutToken = new CancellationTokenSource();
                waitTimeoutToken.CancelAfterSlim(TimeSpan.FromMilliseconds(DownloadUtil.Timeout.Setup));
                var waitLinkedToken = CancellationTokenSource.CreateLinkedTokenSource(token, waitTimeoutToken.Token);

                tcpListener.Start();

                await UniTask.WaitWhile(() => !tcpListener.IsActive, PlayerLoopTiming.Update, waitLinkedToken.Token);

                token.ThrowIfCancellationRequested();

                var acceptTimeoutToken = new CancellationTokenSource();
                acceptTimeoutToken.CancelAfterSlim(TimeSpan.FromMilliseconds(DownloadUtil.Timeout.Read));
                var acceptLinkedToken = CancellationTokenSource.CreateLinkedTokenSource(token, acceptTimeoutToken.Token);

                receiveResult = await tcpListener.AcceptTcpClientAsync()
                                .AsUniTask()
                                .AttachExternalCancellation(acceptLinkedToken.Token);

                token.ThrowIfCancellationRequested();

                if (receiveResult != null)
                {
                    if (receiveResult.Client != null && receiveResult.Client.LingerState != null)
                    {
                        receiveResult.Client.LingerState.Enabled = false;
                        receiveResult.Client.LingerState.LingerTime = 0;
                    }

                    streamResponse = receiveResult.GetStream();
                }

                if (streamResponse == null)
                    throw new Exception("stream not found.");

                token.ThrowIfCancellationRequested();

                var resultLength = 0;
                var resultBytes = new byte[DownloadUtil.ReceiveResponseBuffer];
                var writeTimeout = 0;

                memoryResponse = new MemoryStream();

                _onProgressObserve?.Dispose();
                _onProgressObserve = memoryResponse.ObserveEveryValueChanged(x => x.Length)
                                        .Subscribe(length => DownloadProgress?.UpdateData(length));

                do
                {
                    try
                    {
                        var readTimeoutToken = new CancellationTokenSource();
                        readTimeoutToken.CancelAfterSlim(TimeSpan.FromMilliseconds(DownloadUtil.Timeout.Read));
                        var readLinkedToken = CancellationTokenSource.CreateLinkedTokenSource(token, readTimeoutToken.Token);

                        resultLength = await streamResponse.ReadAsync(resultBytes, 0, resultBytes.Length)
                                                           .AsUniTask()
                                                           .AttachExternalCancellation(readLinkedToken.Token);

                        token.ThrowIfCancellationRequested();

                        if (resultLength <= 0)
                            break;

                        if (OnReceivedResponseError(resultBytes))
                            throw new Exception($"received response error data.");

                        writeTimeout = DownloadUtil.GetWriteTimeOutMillisecond(resultLength);

                        var writeTimeoutToken = new CancellationTokenSource();
                        writeTimeoutToken.CancelAfterSlim(TimeSpan.FromMilliseconds(writeTimeout <= 0 ? DownloadUtil.Timeout.Write : writeTimeout));
                        var writeLinkedToken = CancellationTokenSource.CreateLinkedTokenSource(token, writeTimeoutToken.Token);

                        await memoryResponse.WriteAsync(resultBytes, 0, resultLength > 0 ? resultLength : resultBytes.Length)
                                .AsUniTask()
                                .AttachExternalCancellation(writeLinkedToken.Token);

                        token.ThrowIfCancellationRequested();
                    }
                    catch (Exception ex)
                    {
                        Log(DebugerLogType.Info, "OnReceivedFileDataAsync", $"{ex.Message}");

                        memoryResponse?.Dispose();
                        streamResponse?.Dispose();

                        memoryResponse = null;
                        break;
                    }
                }
                while (streamResponse.DataAvailable);

                var data = memoryResponse?.GetBuffer();
                var result = memoryResponse != null ? new byte[memoryResponse.Length] : null;
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = data[i];
                }

                if (result == null)
                    throw new Exception($"download data error. {info.DataName}|{info.DataLength}");

                if (result.Length != info.DataLength)
                    throw new Exception($"download data error. {info.DataName}|{(result != null ? result.Length : 0)}/{info.DataLength}");

                // ファイルのストレージ書込み
                if (!await OnFileWriteAsync(token, info, outputPath, result))
                    throw new Exception($"file write error.  {info.DataName}|{(result != null ? result.Length : 0)}/{info.DataLength}, outputPath={outputPath}");

                return true;
            }
            catch (Exception ex)
            {
                //Log(DebugerLogType.Info, "OnReceivedFileDataAsync", $"{ex.Message}");

                receiveResult?.Dispose();
                return false;
            }
            finally
            {
                _onProgressObserve?.Dispose();

                memoryResponse?.Close();
                streamResponse?.Close();
                receiveResult?.Close();
                tcpListener?.Stop();
            }
        }

        private bool IsFileLocked(string path)
        {
            FileStream stream = null;

            try
            {
                stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                return false;
            }
            catch
            {
                return true;
            }
            finally
            {
                stream?.Close();
            }
        }

        private async UniTask<bool> OnFileWriteAsync(CancellationToken token, DownloadResponseInfo info, string outputPath, byte[] data)
        {
            FileStream fileStream = null;
            CancellationTokenSource writeTimeoutToken = null;
            CancellationTokenSource writeLinkedTokenSource = null;

            try
            {
                if (!Directory.Exists(outputPath))
                    Directory.CreateDirectory(outputPath);

                fileStream = new FileStream($"{outputPath}/{info.DataName}", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);

                if (fileStream == null)
                    throw new Exception("Create fileStream error.");

                writeTimeoutToken = new CancellationTokenSource();
                writeTimeoutToken.CancelAfterSlim(TimeSpan.FromMilliseconds(DownloadUtil.GetWriteTimeOutMillisecond(data.Length)));
                writeLinkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, writeTimeoutToken.Token);

                await fileStream.WriteAsync(data, 0, data.Length)
                        .AsUniTask()
                        .AttachExternalCancellation(writeLinkedTokenSource.Token);

                fileStream.Close();

                writeTimeoutToken = new CancellationTokenSource();
                writeTimeoutToken.CancelAfterSlim(TimeSpan.FromMilliseconds(DownloadUtil.Timeout.WriteFileReadLock));
                writeLinkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, writeTimeoutToken.Token);

                await UniTask.WaitUntil(() => !IsFileLocked($"{outputPath}/{info.DataName}"), PlayerLoopTiming.Update, writeLinkedTokenSource.Token);

                if (File.Exists($"{outputPath}/{info.DataName}"))
                {
                    // 更新日を合わせる
                    File.SetLastWriteTimeUtc($"{outputPath}/{info.DataName}", info.DataTimestamp);
                }

                return true;
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnFileWriteAsync", $"file={outputPath}/{info.DataName}\n\r{ex.Message}");
                return false;
            }
            finally
            {
                fileStream?.Close();
            }
        }

        protected async UniTask<DownloadResponseInfo> OnSendRequestInfoAsync(CancellationToken token, DownloadRequestOrder request)
        {
            try
            {
                request.OrderCode = DownloadUtil.OrderCode.DownloadInfo;

                var retryCount = DownloadUtil.RequestRetry;
                var sendRequestBytes = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(request));

                if (sendRequestBytes == null || sendRequestBytes.Length <= 0)
                    throw new Exception($"create request error.");

                do
                {
                    try
                    {
                        if (!await OnSendRequestAsync(token, _ipEndPointRequest, sendRequestBytes))
                            throw new Exception("request send error.");

                        _resultResponseBytes = await OnReceivedResponseAsync(token, ipAddress, responsePort);
                        if (_resultResponseBytes == null || _resultResponseBytes.Length <= 0)
                            throw new Exception("response error.");

                        if (OnReceivedResponseError(_resultResponseBytes))
                        {
                            _resultResponse = null;
                            break;
                        }

                        try
                        {
                            _resultResponse = TryFromJson<DownloadResponseInfo>(_resultResponseBytes);
                            if (_resultResponse == null)
                                throw new Exception();

                            if (_resultResponse.ToDeviceId != myDeviceId || _resultResponse.DataLength <= 0)
                                throw new Exception();

                            break;
                        }
                        catch
                        {
                            _resultResponse = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        //Log(DebugerLogType.Info, "OnSendRequestInfoAsync", $"{(request != null ? $"{request.OrderType},{request.OrderValue},{request.OrderFile}" : "---")}| {ex.Message}");
                    }

                    retryCount--;

                    await UniTask.Delay(TimeSpan.FromMilliseconds(DownloadUtil.Interval.Request), cancellationToken: token);
                    token.ThrowIfCancellationRequested();
                }
                while (retryCount > 0);

                return _resultResponse;
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnSendRequestInfoAsync", $"{(request != null ? $"{request.OrderType},{request.OrderValue},{request.OrderFile}" : "---")}| {ex.Message}");
                return null;
            }
        }

        protected async UniTask<bool> OnSendRequestDataAsync(CancellationToken token, DownloadRequestOrder request, DownloadResponseInfo info, string outputPath)
        {
            try
            {
                if (request == null || info == null || string.IsNullOrEmpty(outputPath))
                    throw new Exception($"file info error. outputPath={outputPath}");

                request.OrderCode = DownloadUtil.OrderCode.DownloadData;

                var retryCount = DownloadUtil.RequestRetry;
                var sendRequestBytes = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(request));

                if (sendRequestBytes == null || sendRequestBytes.Length <= 0)
                    throw new Exception($"create request error.");

                do
                {
                    try
                    {
                        if (!await OnSendRequestAsync(token, _ipEndPointRequest, sendRequestBytes))
                            throw new Exception("request send error.");

                        if (!await OnReceivedFileDataAsync(token, ipAddress, responsePort, info, outputPath))
                            throw new Exception("response error.");

                        return true;
                    }
                    catch (Exception ex)
                    {
                        //Log(DebugerLogType.Info, "OnSendRequestDataAsync", $"{(request != null ? $"{request.OrderType},{request.OrderValue},{request.OrderFile}" : "---")}| {ex.Message}");
                    }

                    retryCount--;

                    await UniTask.Delay(TimeSpan.FromMilliseconds(DownloadUtil.Interval.Request), cancellationToken: token);
                    token.ThrowIfCancellationRequested();
                }
                while (retryCount > 0);

                return false;
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnSendRequestDataAsync", $"{(request != null ? $"{request.OrderType},{request.OrderValue},{request.OrderFile}" : "---")}| {ex.Message}");
                return false;
            }
        }

        private bool OnReceivedResponseError(byte[] resultBytes)
        {
            try
            {
                return OnResponseError(TryFromJson<DownloadResponseError>(resultBytes));
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnReceivedResponseError", $"{ex.Message}");
                return false;
            }
        }

        private bool OnResponseError(DownloadResponseError response)
        {
            if (response == null || response.ErrorCode == DownloadUtil.ErrorCode.None)
                return false;

            Log(DebugerLogType.Info, "OnResponseError", $"{response.ErrorCode}: {response.Message}");
            return true;
        }

        #endregion
    }
}
