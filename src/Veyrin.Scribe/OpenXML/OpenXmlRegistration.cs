using Veyrin.Scribe.Core.Extensions;
using Veyrin.Scribe.Core.Interfaces;
using Veyrin.Scribe.Core.Models;
using Veyrin.Scribe.OpenXML.Engine;

namespace Veyrin.Scribe.OpenXML;


/// <summary>
/// 提供 OpenXml 相關文件引擎（Excel、Word、PowerPoint）的註冊與解除註冊功能。
/// </summary>
public class OpenXmlRegistration : IRegistrationProvider
{
    public static T Create<T>(EngineNames name) where T : IEngine => IRegistrationProvider.Create<T>(name);
    public static void Register()
    {
        RegisterExcel();
        RegisterWord();
        RegisterPowerPoint();
    }

    public static T RegisterAndCreateEngine<T>() where T : IEngine
    {
        throw new NotImplementedException();
    }

    public static T RegisterAndCreateEngine<T>(EngineNames name) where T : IEngine
    {
        switch (name)
        {
            case EngineNames.OPENXLS:
                RegisterExcel();
                break;
            case EngineNames.OPENDOC:
                RegisterWord();
                break;
            case EngineNames.OPENPPT:
                RegisterPowerPoint();
                break;
            default:
                break;
        }
        return Create<T>(name);
    }

    public static void Unregister()
    {
        UnregisterExcel();
        UnregisterWord();
        UnregisterPowerPoint();
    }

    /// <summary>
    /// 註冊 Excel (.xlsx) 文件處理引擎。
    /// </summary>
    public static void RegisterExcel() => DocumentFactory.RegisterEngine(EngineNames.OPENXLS, () => (new ExcelEngine()));
    /// <summary>
    /// 註冊 Word (.docx) 文件處理引擎。
    /// </summary>
    public static void RegisterWord() => DocumentFactory.RegisterEngine(EngineNames.OPENDOC, () => (new WordEngine()));
    /// <summary>
    /// 註冊 PowerPoint (.pptx) 文件處理引擎。
    /// </summary>
    public static void RegisterPowerPoint() => DocumentFactory.RegisterEngine(EngineNames.OPENPPT, () => (new PptEngine()));
    /// <summary>
    /// 解除 Excel 文件處理引擎的註冊。
    /// </summary>
    public static void UnregisterExcel() => DocumentFactory.UnRegisterEngine(EngineNames.OPENXLS);
    /// <summary>
    /// 解除 Word 文件處理引擎的註冊。
    /// </summary>
    public static void UnregisterWord() => DocumentFactory.UnRegisterEngine(EngineNames.OPENDOC);
    /// <summary>
    /// 解除 PowerPoint 文件處理引擎的註冊。
    /// </summary>
    public static void UnregisterPowerPoint() => DocumentFactory.UnRegisterEngine(EngineNames.OPENPPT);
}

//public static dynamic Create2(EngineName name) => DocumentFactory.CreateEngine(name);