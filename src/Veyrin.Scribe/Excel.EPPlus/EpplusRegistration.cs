using Veyrin.Scribe.Core.Extensions;
using Veyrin.Scribe.Core.Interfaces;
using Veyrin.Scribe.Core.Models;

namespace Veyrin.Scribe.Excel.EPPlus;

public class EpplusRegistration : IRegistrationProvider
{
    public static T Create<T>() where T : IEngine => IRegistrationProvider.Create<T>(EngineNames.EPPLUS);

    
    public static void Register() => DocumentFactory.RegisterEngine(EngineNames.EPPLUS, () => new ExcelEngine());

    public static T RegisterAndCreateEngine<T>() where T : IEngine => RegisterAndCreateEngine<T>(EngineNames.EPPLUS);

    public static T RegisterAndCreateEngine<T>(EngineNames name) where T : IEngine
    {
        if (name != EngineNames.EPPLUS)
            throw new InvalidOperationException($"Engine '{name}' not registered.");
        Register();
        return IRegistrationProvider.Create<T>(EngineNames.EPPLUS);
    }

    public static void Unregister() => DocumentFactory.UnRegisterEngine(EngineNames.EPPLUS);
}