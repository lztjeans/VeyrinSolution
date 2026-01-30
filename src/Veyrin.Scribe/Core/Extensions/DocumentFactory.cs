using Veyrin.Scribe.Core.Interfaces;
using Veyrin.Scribe.Core.Models;

namespace Veyrin.Scribe.Core.Extensions;

public static class DocumentFactory
{
    private static readonly Dictionary<EngineNames, Func<IEngine>> _registeredEngines = [];
    public static void RegisterEngine(EngineNames name, Func<IEngine> engineFactory) => _registeredEngines[name] = engineFactory;
    public static IEngine CreateEngine(EngineNames name)
    {
        if (name == EngineNames.NONE) throw new ArgumentNullException(nameof(name));
        if (!_registeredEngines.TryGetValue(name, out var factory))
            throw new InvalidOperationException($"Engine '{name}' not registered.");
        return factory();
    }
    public static void UnRegisterEngine(EngineNames name) => _registeredEngines.Remove(name);
    public static Dictionary<EngineNames, Func<IEngine>> RegisteredEngineLists() => _registeredEngines;

}