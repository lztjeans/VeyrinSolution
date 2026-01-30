using System.Net.NetworkInformation;

namespace Veyrin.Core.Network
{
    public static class PortUtils
    {
        /// <summary>
        /// 檢查指定的 TCP Port 是否正在被使用
        /// </summary>
        public static bool IsPortInUse(int port)
        {
            var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            var tcpConnections = ipGlobalProperties.GetActiveTcpListeners();

            return tcpConnections.Any(endpoint => endpoint.Port == port);
        }

        /// <summary>
        /// 尋找下一個可用的 TCP Port (從指定起始值開始)
        /// </summary>
        public static int FindAvailablePort(int startPort = 1000)
        {
            for (int port = startPort; port <= 65535; port++)
            {
                if (!IsPortInUse(port)) return port;
            }
            throw new Exception("No available TCP ports found.");
        }
    }
}
