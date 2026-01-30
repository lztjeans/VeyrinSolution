using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veyrin.Core.Diagnostics
{
    public static class VeyrinRuntime
    {
        private static readonly ConcurrentDictionary<string, bool> _moduleStatus = new();

        public static void MarkModuleReady(string moduleName)
            => _moduleStatus[moduleName] = true;

        public static bool IsModuleReady(string moduleName)
            => _moduleStatus.TryGetValue(moduleName, out var ready) && ready;
    }
}
