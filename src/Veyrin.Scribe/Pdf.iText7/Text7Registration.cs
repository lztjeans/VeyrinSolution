using Veyrin.Scribe.Core.Extensions;
using Veyrin.Scribe.Core.Interfaces;
using Veyrin.Scribe.Core.Models;

namespace Veyrin.Scribe.Pdf.iText7;

public class Text7Registration : IRegistrationProvider
{
    public static T Create<T>() where T : IEngine => IRegistrationProvider.Create<T>(EngineNames.TEXT7);

    public static void Register() => DocumentFactory.RegisterEngine(EngineNames.TEXT7, () => new PdfEngine());


    public static T RegisterAndCreateEngine<T>() where T : IEngine => RegisterAndCreateEngine<T>(EngineNames.TEXT7);

    public static T RegisterAndCreateEngine<T>(EngineNames name) where T : IEngine
    {
        if (name != EngineNames.TEXT7)
            throw new InvalidOperationException($"Engine '{name}' not registered.");
        Register();
        return IRegistrationProvider.Create<T>(EngineNames.TEXT7);
    }

    public static void Unregister() => DocumentFactory.UnRegisterEngine(EngineNames.TEXT7);
}