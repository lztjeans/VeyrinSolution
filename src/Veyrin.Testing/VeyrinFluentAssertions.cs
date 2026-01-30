using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Veyrin.Core.Exceptions;

namespace Veyrin.Testing
{
    public static class VeyrinFluentAssertions
    {
        // 測試 Pulse 的回傳結果
        public static void ShouldBeSuccess(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
                throw new AssertException($"預期成功，但收到: {response.StatusCode}");
        }

        // 測試 Scribe 生成的檔案
        public static void ShouldExistOnDisk(this FileInfo fileInfo)
        {
            fileInfo.Refresh();
            if (!fileInfo.Exists)
                throw new AssertException($"檔案未正確生成: {fileInfo.FullName}");
        }
    }
}
