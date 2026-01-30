using Veyrin.Scribe.Core.Extensions;
using Veyrin.Scribe.Core.Interfaces;
using Veyrin.Scribe.Core.Models;

namespace Veyrin.Scribe.Text.Csv;

public class CsvRegistration : IRegistrationProvider
{
    public static T Create<T>() where T : IEngine => IRegistrationProvider.Create<T>(EngineNames.CSV);

    public static void Register() => DocumentFactory.RegisterEngine(EngineNames.CSV, () => new CsvEngine());

    public static T RegisterAndCreateEngine<T>() where T : IEngine => RegisterAndCreateEngine<T>(EngineNames.CSV);

    public static T RegisterAndCreateEngine<T>(EngineNames name) where T : IEngine
    {
        if (name != EngineNames.CSV)
            throw new InvalidOperationException($"Engine '{name}' not registered.");
        Register();
        return IRegistrationProvider.Create<T>(EngineNames.CSV);
    }

    public static void Unregister() => DocumentFactory.UnRegisterEngine(EngineNames.CSV);
}