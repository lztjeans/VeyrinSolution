using Veyrin.Scribe.Excel.ClosedXML;

namespace Veyrin.Demo.Scribe.ClosedXml;

public static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Main stat");
        //ClosedXmlRegistration.Register();
        //var eng = ClosedXmlRegistration.Create<ExcelEngine>();
        //eng.CreateXLSXWorkbook();


        var e = ClosedXmlRegistration.RegisterAndCreateEngine<ExcelEngineV1>();
        e.CreateXLSXWorkbook().AddWorksheet("");
        e.AddWorksheet("S1").AddWorksheet("");
        //Console.WriteLine("Press any key to continue.");
        //Console.ReadKey();
        Console.WriteLine("Main end");
        var e2 = ClosedXmlRegistration.RegisterAndCreateEngine<ExcelEngineV2>();
        e2.CreateXLS();
        e2.Add("").SetActive("");e2.SetFormula("", 0, 0, "");

    }
}
