using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veyrin.Core.Thread
{
    public static class ConcurrencyUtils
    {
        private static DateTime _lastRun = DateTime.MinValue;

        /// <summary>
        /// 簡單節流：若距離上次執行不足 interval，則不執行
        /// </summary>
        public static void Throttle(Action action, TimeSpan interval)
        {
            if (DateTime.Now - _lastRun > interval)
            {
                action();
                _lastRun = DateTime.Now;
            }
        }
    }
}
