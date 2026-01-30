using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veyrin.Core.Thread
{
    public static class TaskExtensions
    {
        /// <summary>
        /// 為 Task 增加超時限制
        /// </summary>
        public static async Task WithTimeout(this Task task, TimeSpan timeout)
        {
            using var delayTaskCts = new CancellationTokenSource();
            var delayTask = Task.Delay(timeout, delayTaskCts.Token);

            var completedTask = await Task.WhenAny(task, delayTask).ConfigureAwait(false);
            if (completedTask == delayTask)
                throw new TimeoutException("The operation has timed out.");

            delayTaskCts.Cancel();
            await task; // 傳遞原始 Task 的異常
        }

        /// <summary>
        /// 安全地執行背景任務，防止未捕獲異常導致進程崩潰
        /// </summary>
        public static void Forget(this Task task, Action<Exception>? onException = null)
        {
            task.ContinueWith(t =>
            {
                if (t.IsFaulted && onException != null)
                    onException(t.Exception!.InnerException ?? t.Exception);
            }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
