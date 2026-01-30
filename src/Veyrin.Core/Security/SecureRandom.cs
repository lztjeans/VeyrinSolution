using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Veyrin.Core.Security
{
    public static class SecureRandom
    {
        /// <summary>
        /// 產生指定長度的隨機字串 (常用於密碼鹽、Token)
        /// </summary>
        public static string GenerateString(int length = 32)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var result = new char[length];
            using var rng = RandomNumberGenerator.Create();

            byte[] data = new byte[length];
            rng.GetBytes(data);

            for (int i = 0; i < length; i++)
            {
                result[i] = chars[data[i] % chars.Length];
            }
            return new string(result);
        }

        /// <summary>
        /// 產生安全的隨機整數
        /// </summary>
        public static int Next(int min, int max)
        {
            return RandomNumberGenerator.GetInt32(min, max);
        }
    }
}
