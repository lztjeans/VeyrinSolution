using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Veyrin.Core.Logging;

namespace Veyrin.Core.Diagnostics;

public sealed class VeyrinProfiler : IDisposable
{
    private readonly string _name;
    private readonly Stopwatch _sw;
    private readonly ILogger _logger;

    public VeyrinProfiler(string name)
    {
        _name = name;
        //_logger = VeyrinLoggerFactory.CreateLogger<VeyrinProfiler>();
        _logger = AppLogger.For<VeyrinProfiler>();
        _sw = Stopwatch.StartNew();
    }

    public void Dispose()
    {
        _sw.Stop();
        _logger.LogInformation($"[Profiler] {_name} 耗時: {_sw.ElapsedMilliseconds} ms");
    }
}
