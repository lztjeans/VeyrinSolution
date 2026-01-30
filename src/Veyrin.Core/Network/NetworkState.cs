using System.Net.NetworkInformation;

namespace Veyrin.Core.Network
{
    public static class NetworkState
    {
        /// <summary>
        /// 檢查本機是否有網路連線 (透過網路卡狀態判斷)
        /// </summary>
        public static bool IsConnected => NetworkInterface.GetIsNetworkAvailable();

        /// <summary>
        /// 透過 Ping 指定主機檢查網路是否真的「通暢」
        /// </summary>
        public static async Task<bool> CanPingAsync(string host = "8.8.8.8", int timeout = 2000)
        {
            try
            {
                using var ping = new Ping();
                var reply = await ping.SendPingAsync(host, timeout);
                return reply.Status == IPStatus.Success;
            }
            catch
            {
                return false;
            }
        }
    }
}
