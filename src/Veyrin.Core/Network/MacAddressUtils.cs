using System.Net.NetworkInformation;

namespace Veyrin.Core.Network
{
    public static class MacAddressUtils
    {
        /// <summary>
        /// 取得第一個有效網卡的 MAC 位址 (格式：XX:XX:XX:XX:XX:XX)
        /// </summary>
        public static string GetActiveMacAddress()
        {
            return NetworkInterface.GetAllNetworkInterfaces()
                .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .Select(nic => nic.GetPhysicalAddress().ToString())
                .FirstOrDefault() ?? string.Empty;
        }
    }
}
