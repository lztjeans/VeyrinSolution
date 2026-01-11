//using ExcelToolkit.Extensions;
//using ExcelToolkit.Models;

//ClosedXmlRegistration.Register();

//var eng = ExcelHelperFactory.CreateEngine(new ExcelOptions
//{
//    Engine = "ClosedXML"
//});
//eng.CreateWorkbook()
//    .AddWorksheet("Users")
//.SetCellValue(1, 1, "ID")
//.SetCellValue("Users", 1, 2, "Name")
//.SetCellValue("Users", 1, 3, "Age")
//.SetCellValue("Users", 2, 1, 1)
//.SetCellValue("Users", 2, 2, "John")
//.SetCellValue("Users", 2, 3, 30)
//.MergeCells(3, 1, 3, 3)
//.SetCellStyle("Users", 1, 1, new DocumentFontStyle
//{
//    Bold = true,
//    BackgroundColor = "#007ACC",
//    FontColor = "#FFFFFF",
//    HorizontalAlign = "center"
//})
//.SaveToFile(@"D:\output\users.xlsx");

//OpenXmlRegistration.Register();
//OpenXmlEngine eng = (OpenXmlEngine)DocumentFactory.CreateEngine(new ExcelOptions { Engine = "OpenXML" });

//eng.CreateWorkbook()
//    .AddWorksheet("Users")
//.SetCellValue(1, 1, "ID")
//.SetCellValue("Users", 1, 2, "Name")
//.SetCellValue("Users", 1, 3, "Age")
//.SetCellValue("Users", 2, 1, 1)
//.SetCellValue("Users", 2, 2, "John")
//.SetCellValue("Users", 2, 3, 30)
//.MergeCells(3, 1, 3, 3)
//.SetCellStyle("Users", 1, 1, new DocumentFontStyle
//{
//    Bold = true,
//    BackgroundColor = "#007ACC",
//    FontColor = "#FFFFFF",
//    HorizontalAlign = "center"
//})
//.SaveToFile(@"D:\output\users.xlsx");

using DocumentToolkit;
//CSV();

Npoi();

static void CSV() {
    CsvRegistration.Register();
    CsvContext ctx = (CsvContext)DocumentFactory.CreateEngine(name: "CSV");
    ctx.CreateFile();
    //ctx.LoadFile("");
    ctx.WriteText("asbd,e,56");
    ctx.SaveToFile(@"D:\output\users.csv");

    //var npoi = ctx.GetEngine(engineName: "");
    Console.WriteLine("pause");
    Console.WriteLine(ctx.ReadText());
}

static void Npoi()
{
    NpoiRegistration.RegisterExcel();
    //IFileDocumentContext ctx = DocumentFactory.CreateEngine(name: "NPOI");
    ExcelContext ctx = (ExcelContext)DocumentFactory.CreateEngine(name: "NPOI_Xls");
    ctx.CreateWorkbook();
    ctx.AddWorksheet("Users");
    ctx.SetCellValue("Users", 1, 1, "ID");
    ctx.SetCellValue("Users", 1, 2, "Name");
    ctx.SetCellValue("Users", 1, 3, "Age");
    ctx.SetCellValue("Users", 2, 1, 1);
    ctx.SetCellValue("Users", 2, 2, "John");
    ctx.SetCellValue("Users", 2, 3, 30);
    ctx.MergeCells("Users", 3, 1, 3, 3);
    ctx.SetCellStyle("Users", 1, 1, new DocumentFontStyle
    {
        Bold = true,
        FontColor = "#FFFFFF",
        BackgroundColor = "#007ACC",
        HorizontalAlign = "center"
    });
    ctx.SaveToFile(@"D:\output\users_npoi.xlsx");
    //NpoiEngine eng = (NpoiEngine)ctx.GetEngine();
    
}