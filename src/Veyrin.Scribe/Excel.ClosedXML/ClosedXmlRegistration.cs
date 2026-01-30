using Veyrin.Scribe.Core.Extensions;
using Veyrin.Scribe.Core.Interfaces;
using Veyrin.Scribe.Core.Models;

namespace Veyrin.Scribe.Excel.ClosedXML;

public class ClosedXmlRegistration : IRegistrationProvider
{
    public static T Create<T>() where T : IEngine => IRegistrationProvider.Create<T>(EngineNames.ClosedXML);

    public static void Register() => DocumentFactory.RegisterEngine(EngineNames.ClosedXML, () => (new ExcelEngineV1()));
    public static void Unregister() => DocumentFactory.UnRegisterEngine(EngineNames.ClosedXML);

    public static T RegisterAndCreateEngine<T>(EngineNames name) where T : IEngine
    {
        if (name != EngineNames.ClosedXML)
            throw new InvalidOperationException($"Engine '{name}' not registered.");
        Register();
        return IRegistrationProvider.Create<T>(EngineNames.ClosedXML);
    }

    public static T RegisterAndCreateEngine<T>() where T : IEngine => RegisterAndCreateEngine<T>(EngineNames.ClosedXML);
}