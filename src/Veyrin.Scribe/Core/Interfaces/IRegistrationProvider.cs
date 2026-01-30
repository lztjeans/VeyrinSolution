using Veyrin.Scribe.Core.Extensions;
using Veyrin.Scribe.Core.Models;

namespace Veyrin.Scribe.Core.Interfaces;

public interface IRegistrationProvider
{
    /// <summary>一鍵註冊所有引擎。</summary>
    public abstract static void Register();
    /// <summary>一鍵解除所有已註冊的引擎。</summary>
    public abstract static void Unregister();

    /// <summary> 取得實例</summary>
    public static T Create<T>(EngineNames name) where T : IEngine
    {
        var instance = DocumentFactory.CreateEngine(name);
        if (instance is T result) return result;
        throw new InvalidCastException($"引擎 '{name}' 生產出的類型是 {instance.GetType().Name}，無法轉換為要求的 {typeof(T).Name}");
    }
    /// <summary> 一次完成註冊並取得實例</summary>
    public abstract static T RegisterAndCreateEngine<T>() where T : IEngine;
    public abstract static T RegisterAndCreateEngine<T>(EngineNames name) where T : IEngine;
}