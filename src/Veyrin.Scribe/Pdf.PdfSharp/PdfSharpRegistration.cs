using Veyrin.Scribe.Core.Extensions;
using Veyrin.Scribe.Core.Interfaces;
using Veyrin.Scribe.Core.Models;

namespace Veyrin.Scribe.Pdf.PdfSharp;

public class PdfSharpRegistration : IRegistrationProvider
{
    public static T Create<T>() where T : IEngine => IRegistrationProvider.Create<T>(EngineNames.PDF);

    public static void Register() => DocumentFactory.RegisterEngine(EngineNames.PDF, () => new PdfEngine());


    public static T RegisterAndCreateEngine<T>() where T : IEngine => RegisterAndCreateEngine<T>(EngineNames.PDF);

    public static T RegisterAndCreateEngine<T>(EngineNames name) where T : IEngine
    {
        if (name != EngineNames.PDF)
            throw new InvalidOperationException($"Engine '{name}' not registered.");
        Register();
        return IRegistrationProvider.Create<T>(EngineNames.PDF);
    }

    public static void Unregister() => DocumentFactory.UnRegisterEngine(EngineNames.PDF);
}
