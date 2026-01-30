namespace Veyrin.Scribe.Core.Models;
public class DocumentOptions
{
    public string Engine { get; set; } = "ClosedXML"; // 預設
    public string DocumentType { get; set; } = string.Empty; // xls/xlsx/doc/docx/pdf/csv/txt ...
    public string DocumentName { get; set; } = string.Empty;

}
