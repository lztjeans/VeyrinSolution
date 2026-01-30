using Veyrin.Scribe.Core.Extensions;
using Veyrin.Scribe.Core.Interfaces;
using Veyrin.Scribe.Core.Models;

namespace Veyrin.Scribe.NPOI;

public class NpoiRegistration : IRegistrationProvider
{
    public static T Create<T>(EngineNames name) where T : IEngine => IRegistrationProvider.Create<T>(name);

    public static void Register()
    {
        RegisterExcel();
        RegisterWord();
        RegisterPowerPoint();
    }
    public static void RegisterExcel()
    {
        DocumentFactory.RegisterEngine(EngineNames.NPOIXLS, () => (new ExcelEngine()));
    }
    public static void RegisterWord()
    {
        DocumentFactory.RegisterEngine(EngineNames.NPOIDOC, () => (new WordEngine()));
    }
    public static void RegisterPowerPoint()
    {
        DocumentFactory.RegisterEngine(EngineNames.NPOIPPT, () => (new PptEngine()));
    }
    public static void Unregister()
    {
        UnregisterExcel();
        UnregisterWord();
        UnregisterPowerPoint();
    }
    public static void UnregisterExcel() => DocumentFactory.UnRegisterEngine(EngineNames.NPOIXLS);
    public static void UnregisterWord() => DocumentFactory.UnRegisterEngine(EngineNames.NPOIDOC);
    public static void UnregisterPowerPoint() => DocumentFactory.UnRegisterEngine(EngineNames.NPOIPPT);

    public static T RegisterAndCreateEngine<T>() where T : IEngine
    {
        throw new NotImplementedException();
    }

    public static T RegisterAndCreateEngine<T>(EngineNames name) where T : IEngine
    {
        switch (name)
        {
            case EngineNames.NPOIXLS:
                RegisterExcel();
                break;
            case EngineNames.NPOIDOC:
                RegisterWord();
                break;
            case EngineNames.NPOIPPT:
                RegisterPowerPoint();
                break;
            default:
                break;
        }
        return Create<T>(name);
    }
}