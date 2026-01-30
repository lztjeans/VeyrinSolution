using System.Net;
using System.Net.Sockets;

namespace Veyrin.Core.Network
{
    public static class IPAddressUtils
    {
        /// <summary>
        /// 取得本機所有的 IPv4 位址 (排除迴路位址 127.0.0.1)
        /// </summary>
        public static IEnumerable<IPAddress> GetLocalIpv4Addresses()
        {
            return Dns.GetHostEntry(Dns.GetHostName())
                .AddressList
                .Where(ip => ip.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip));
        }

        /// <summary>
        /// 判斷 IP 是否為私有網路位址 (Private IP, 如 192.168.x.x)
        /// </summary>
        public static bool IsPrivateIp(this IPAddress ip)
        {
            byte[] bytes = ip.GetAddressBytes();
            return ip.AddressFamily == AddressFamily.InterNetwork && bytes[0] switch
            {
                10 => true,
                172 => bytes[1] >= 16 && bytes[1] <= 31,
                192 => bytes[1] == 168,
                _ => false
            };
        }
    }
}
