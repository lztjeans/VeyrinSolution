using System.Data;

namespace Veyrin.Scribe.Core.Interfaces.Excel
{
    public interface IExcelContentEditor
    {
        // 指定 Sheet
        void SetValue(string sheetName, int row, int col, object value);
        object? GetValue(string sheetName, int row, int col);

        // 批次與搜尋
        void ImportDataTable(string sheetName, int startRow, int startCol, DataTable dt, bool headers);
        Tuple<int, int>? Find(string sheetName, string text);
        int GetLastRowIndex(string sheetName);

        // 進階內容
        void SetFormula(string sheetName, int row, int col, string formula);
        void InsertImage(string sheetName, byte[] data, int row, int col, int w, int h);
    }
}
