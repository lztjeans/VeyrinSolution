using System.Security.Cryptography;
using System.Text;

namespace Veyrin.Core.Security
{
    public static class HashUtils
    {
        /// <summary>
        /// 計算字串的 SHA256 雜湊值 (回傳 64 位元小寫十六進位字串)
        /// </summary>
        public static string ToSha256(this string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return ToHexString(bytes);
        }

        /// <summary>
        /// 計算 MD5 (僅建議用於檔案校驗或舊版相容)
        /// </summary>
        public static string ToMd5(this string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            using var md5 = MD5.Create();
            byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return ToHexString(bytes);
        }

        private static string ToHexString(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (var b in bytes)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}
