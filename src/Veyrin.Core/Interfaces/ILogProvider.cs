using Veyrin.Core.Models;

namespace Veyrin.Core.Interfaces;

/// <summary>
/// Log 實作介面：外部專案需實作此介面並注入給 LogHelper
/// </summary>
public interface ILogProvider
{
    void Log(LogLevelEnum level, string message, Exception? ex = null, params object[] args);
}
