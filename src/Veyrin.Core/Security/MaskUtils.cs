using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veyrin.Core.Security
{
    public static class MaskUtils
    {
        /// <summary>
        /// 遮蔽 Email (例如: v***n@example.com)
        /// </summary>
        public static string MaskEmail(this string email)
        {
            if (string.IsNullOrEmpty(email) || !email.Contains("@")) return email;
            var parts = email.Split('@');
            if (parts[0].Length <= 2) return $"{parts[0][0]}***@{parts[1]}";
            return $"{parts[0].Substring(0, 2)}***{parts[0][^1]}@{parts[1]}";
        }
    }
}
