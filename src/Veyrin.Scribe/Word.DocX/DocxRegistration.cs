using Veyrin.Scribe.Core.Extensions;
using Veyrin.Scribe.Core.Interfaces;
using Veyrin.Scribe.Core.Models;

namespace Veyrin.Scribe.Word.DocX;
public class DocxRegistration : IRegistrationProvider
{
    public static T Create<T>() where T : IEngine => IRegistrationProvider.Create<T>(EngineNames.DOCX);

    public static void Register() => DocumentFactory.RegisterEngine(EngineNames.DOCX, () => (new WordEngine()));

    public static T RegisterAndCreateEngine<T>() where T : IEngine => RegisterAndCreateEngine<T>(EngineNames.DOCX);

    public static T RegisterAndCreateEngine<T>(EngineNames name) where T : IEngine
    {
        if (name != EngineNames.DOCX)
            throw new InvalidOperationException($"Engine '{name}' not registered.");
        Register();
        return IRegistrationProvider.Create<T>(EngineNames.DOCX);
    }

    public static void Unregister() => DocumentFactory.UnRegisterEngine(EngineNames.DOCX);
}