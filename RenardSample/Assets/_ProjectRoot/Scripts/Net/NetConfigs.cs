using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.NetworkInformation;
using Cysharp.Threading.Tasks;

namespace SignageHADO.Net
{
    using Renard.Debuger;

    public class NetConfigs
    {
        private static void Log(DebugerLogType logType, string methodName, string message)
        {
            DebugLogger.Log(typeof(NetConfigs), logType, methodName, message);
        }

        private static List<IPAddress> _addressList = new List<IPAddress>();
        public static IPAddress[] GetAddressList(bool linkLocalAddress = false)
        {
            _addressList.Clear();

            try
            {
                foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        if (ni.Name != "lo" && ni.Name != "lo0")
                        {
                            if (linkLocalAddress)
                            {
                                _addressList.AddRange(ni.GetIPProperties().UnicastAddresses
                                                        .Select(x => x.Address)
                                                        .ToArray());
                            }
                            else
                            {
                                _addressList.AddRange(ni.GetIPProperties().UnicastAddresses
                                                        .Select(x => x.Address)
                                                        .Where(x => x.ToString().IndexOf("169.254") == -1)
                                                        .ToArray());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "addressList", $"Error. {ex.Message}");
            }

            return _addressList.ToArray();
        }

        private static List<IPAddress> _gatewayList = new List<IPAddress>();
        public static IPAddress[] GetGatewayList(bool linkLocalAddress = false)
        {
            _gatewayList.Clear();

            try
            {
                foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.OperationalStatus != OperationalStatus.Up) continue;
                    if (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback) continue;

                    if (linkLocalAddress)
                    {
                        _gatewayList.AddRange(ni.GetIPProperties().GatewayAddresses
                                                .Select(x => x.Address)
                                                .ToArray());
                    }
                    else
                    {
                        _gatewayList.AddRange(ni.GetIPProperties().GatewayAddresses
                                                .Select(x => x.Address)
                                                .Where(x => x.ToString().IndexOf("169.254") == -1)
                                                .ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "gatewayList", $"Error. {ex.Message}");
            }

            return _gatewayList.ToArray();
        }

        public static string GetDefaultGatewayIPv4(bool linkLocalAddress = false) => GetGatewayList(linkLocalAddress).FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.ToString();

        public static string GetIP(bool linkLocalAddress = false) => GetAddressList(linkLocalAddress).FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)?.ToString();

        public static string GetIPv4(bool linkLocalAddress = false) => GetAddressList(linkLocalAddress).FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.ToString();

        public static string GetNetworkIPv4(string hostIP)
        {
            var ipSplit = hostIP.Split('.');
            if (ipSplit.Length == 4)
            {
                foreach (var address in GetAddressList())
                {
                    if (address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                        continue;

                    if (Regex.IsMatch(address.ToString(), $"{ipSplit[0]}.{ipSplit[1]}.{ipSplit[2]}.[0-9]*"))
                        return address.ToString();
                }
            }
            return "127.0.0.0";
        }

        private static IPAddress _parseAddress = null;

        public static bool CheckedStringAddress(string address)
        {
            if (!string.IsNullOrEmpty(address))
            {
                if (IPAddress.TryParse(address, out _parseAddress))
                    return true;
            }
            return false;
        }

        private static int _index = 0;
        private static IPAddress[] _getAddressList = null;

        public static bool IsEffectiveAddress(string address)
        {
            if (CheckedStringAddress(address))
            {
                _getAddressList = GetAddressList();
                if (_getAddressList != null && _getAddressList.Length > 0)
                {
                    for (_index = 0; _index < _getAddressList.Length; _index++)
                    {
                        if (_getAddressList[_index].ToString() == address)
                            return true;
                    }
                }
            }
            return false;
        }

        private static IPGlobalProperties _ipGlobalProperties
        {
            get
            {
                try
                {
                    return IPGlobalProperties.GetIPGlobalProperties();
                }
                catch (Exception ex)
                {
                    Log(DebugerLogType.Info, "_ipGlobalProperties", $"GetIPGlobalProperties() {ex.Message}");
                }
                return null;
            }
        }

        public static bool IsAvailablePort(string ipAddress, int port, bool udpListeners = false)
        {
            try
            {
                if (_ipGlobalProperties != null)
                {
                    if (udpListeners)
                    {
                        var udpListenersArray = _ipGlobalProperties.GetActiveUdpListeners();
                        foreach (var udpPort in udpListenersArray)
                        {
                            Log(DebugerLogType.Info, "IsAvailablePort", $"udpPort::ip={udpPort.Address.ToString()},port={udpPort.Port}");
                            if (udpPort.Address.ToString() == ipAddress && udpPort.Port == port)
                                return true;
                        }
                    }
                    else
                    {
                        var tcpListenersArray = _ipGlobalProperties.GetActiveTcpListeners();
                        foreach (var tcpPort in tcpListenersArray)
                        {
                            Log(DebugerLogType.Info, "IsAvailablePort", $"tcpPort::ip={tcpPort.Address.ToString()},port={tcpPort.Port}");
                            if (tcpPort.Address.ToString() == ipAddress && tcpPort.Port == port)
                                return true;
                        }
                    }
                }
            }
            catch (Exception)
            {
                Log(DebugerLogType.Info, "IsAvailablePort", $"_ipGlobalProperties{(_ipGlobalProperties != null ? " != null" : " == null")}");
            }
            return false;
        }

        private static CancellationTokenSource _cancellationToken = null;
        private static Ping _pingSender = null;
        private static PingReply _reply = null;
        private const int _pingSenderTimeout = 50;

        public static void Ping(string ipAddress, Action<bool, string> onCompleted)
        {
            if (string.IsNullOrEmpty(ipAddress))
            {
                if (onCompleted != null)
                    onCompleted(false, "IsEmpty ipAddress.");

                return;
            }

            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _cancellationToken = new CancellationTokenSource();

            try
            {
                UniTask.Void(async (token) =>
                {
                    await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
                    token.ThrowIfCancellationRequested();

                    _pingSender = new Ping();
                    _reply = _pingSender.Send(ipAddress, _pingSenderTimeout);

                    if (onCompleted != null)
                        onCompleted((_reply.Status == IPStatus.Success), $"Reply from {_reply.Address}: bytes={_reply.Buffer.Length}, time={_reply.RoundtripTime}ms, TTL={_reply.Options.Ttl}");
                },
                _cancellationToken.Token);
            }
            catch (Exception ex)
            {
                if (onCompleted != null)
                    onCompleted(false, $"CancellationToken. {ex.Message}");
            }
        }
    }
}
