
public interface IDocumentContext
{
    //void CreateFile();
    //void LoadFile(string path);
    //void SaveToFile(string path);
    //MemoryStream SaveToStream();
    //byte[] SaveToBytes();
    //string ReadText();
    //T Read<T>() where T : class;
    //void WriteText(string content);
    //void Write<T>(T content) where T : class;


    // Excel
    //void CreateWorkbook();
    //void AddWorksheet(string name);
    //void SetCellValue(string sheetName, int row, int column, object value);
    //void MergeCells(string sheetName, int startRow, int startColumn, int endRow, int endColumn);
    //void UnmergeCells(string sheetName, int startRow, int startColumn, int endRow, int endColumn);
    //void SetCellStyle(string sheetName, int row, int column, DocumentFontStyle style);

    // Word / PDF
    //void AddParagraph(string text);
    //void AddTable<T>(IEnumerable<T> data);

    // Native object
    //object GetNativeDocument();
    object GetEngine();
}


//object GetEngine(string engineName);
////Load/Create
//void LoadFromFile(string filePath);
//void Create(DocumentOptions options);

//// Export

