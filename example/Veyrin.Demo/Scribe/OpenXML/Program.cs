// See https://aka.ms/new-console-template for more information
//using Veyrin.Scribe.Core.Contexts;
using Veyrin.Scribe.Core.Models;
using Veyrin.Scribe.OpenXML;
using Veyrin.Scribe.OpenXML.Engine;


namespace Veyrin.Demo.Scribe.OpenXML;

public static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        try
        {
            //OpenXmlRegistration.RegisterWord();
            OpenXmlRegistration.RegisterExcel();
            //WordContext ctx = (WordContext)DocumentFactory.CreateEngine(EngineName.OPENDOC);
            //WordContext ctx = (WordContext)DocumentFactory.RegisterAndCreateEngine(EngineName.OPENDOC, () => new WordContext(new DocEngine()));
            //var ctx_doc = OpenXmlRegistration.RegisterAndCreate<WordContext>(EngineName.CSV);
            //OpenXmlRegistration.RegisterExcel();
            //var ctx_exc = OpenXmlRegistration.Create<ExcelContext>(EngineName.OPENXLS);
            //List<EngineName> list = DocumentFactory.RegisteredEngineLists().Keys.ToList();

            //var xlsEng = OpenXmlRegistration.Create<XlsEngine>(EngineName.OPENXLS);
            // xlsEng.CreateXLSWorkbook();
            ///*
            // */
            //var docEng = OpenXmlRegistration.Create<DocEngine>(EngineName.OPENDOC);
            // docEng.CreateDocument();
            ///*
            // */
            //var pptEng = OpenXmlRegistration.Create<PptEngine>(EngineName.OPENPPT);
            //pptEng.CreatePresentation();

            var x = OpenXmlRegistration.RegisterAndCreateEngine<ExcelEngine>(EngineName.OPENXLS);
            ////var x = OpenXmlRegistration.Create2(EngineName.OPENXLS);

            //x.CreateXLSXWorkbook();


            var x2 = OpenXmlRegistration.Create<ExcelEngine>(EngineName.OPENXLS);
            //x2.CreateXLSXWorkbook();
            //x2.SaveToFile(@$"C:\Shared\ScribeDemo\x2_{DateTime.Now:HHmmss.ffff}.xlsx");
            //Thread.Sleep(30000);
            //x.SaveToFile(@$"C:\Shared\ScribeDemo\x1_{DateTime.Now:HHmmss.ffff}.xlsx");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        //Word();
        //Excel();
        //system pause
        //Print(list);
        Console.WriteLine("press any key to continue.");
        Console.ReadKey();
    }
    private static void Excel()
    {
        //OpenXmlRegistration.RegisterExcel();
        //ExcelContext ctx = (ExcelContext)DocumentFactory.CreateEngine(EngineName.OPENXLS);
        //try
        //{
        //    ctx.CreateXLSWorkbook();
        //    ctx.SaveToFile(@$"C:\Shared\ScribeDemo\testXLS1.xls");
        //}
        //catch (Exception ex)
        //{
        //    Console.Error.WriteLine(ex.ToString());
        //}
        //try
        //{
        //    ctx.CreateXLSXWorkbook();
        //    ctx.SaveToFile(@$"C:\Shared\ScribeDemo\testXLS2.xlsx");
        //}
        //catch (Exception ex)
        //{
        //    Console.Error.WriteLine(ex.ToString());
        //}
        //IExcelEngine eng = (IExcelEngine)ctx.GetEngine();
        //eng.CreateXLSWorkbook();
        //eng.SaveToFile(@$"C:\Shared\ScribeDemo\testXLS1.xls");
        //eng.CreateXLSXWorkbook();
        //eng.SaveToFile(@$"C:\Shared\ScribeDemo\testXLS2.xls");

    }

    private static void Word()
    {
        //OpenXmlRegistration.RegisterWord();
        //OpenXmlRegistration.Register();
        //WordContext ctx = (WordContext)DocumentFactory.CreateEngine(EngineName.OPENDOC);

        //WordContext ctx = (WordContext)DocumentFactory.RegisterAndCreateEngine(EngineName.OPENDOC, () => new WordContext(new DocEngine()));
        //WordContext ctx_doc = OpenXmlRegistration.RegisterAndCreate<WordContext>(EngineName.CSV);

        //OpenXmlRegistration.RegisterExcel();
        //ExcelContext ctx_exc = OpenXmlRegistration.Create<ExcelContext>(EngineName.OPENXLS);

        //CsvRegistration.Register();
        //var ctxxx = DocumentFactory.CreateEngine(EngineName.CSV);

        //var eng1 = ctx_exc.GetEngine();

        //ctx_doc.Create();
        //ctx_doc.WriteLine("asdf");
        //ctx_doc.WriteLine("133");
        //DocEngine eng = (DocEngine)ctx_doc.GetEngine();
        //eng.AppendText("%^#*@#$%&");
        //ctx_doc.WriteLine("%^#*@#$%&........");
        //ctx_doc.Save(@$"C:\Shared\ScribeDemo\testDOC1.docx");
        //eng.CreateDocument();
        //eng.SaveToFile(@$"C:\Shared\ScribeDemo\testDOC1.docx");//pass
        //eng.CreateDocument();
        //eng.SaveToFile(@$"C:\Shared\ScribeDemo\testDOC2.doc");//pass

    }

    private static void Print<T>(IEnumerable<T> values)
    {
        foreach (var item in values)
        {
            Console.WriteLine(item);
        }
    }
}