using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Veyrin.Scribe.Core.Interfaces;
using Veyrin.Scribe.Core.Interfaces.Excel;
using Veyrin.Scribe.Core.Models;
using Veyrin.Scribe.OpenXML.Helper;
namespace Veyrin.Scribe.OpenXML.Engine;
public class ExcelEngine : IExcelEngine, IEngine
{
    private SpreadsheetDocument? _document;
    private WorkbookPart? _workbookPart;
    private string? _activeSheetName;

    // --- 防禦性包裝屬性 ---
    private SpreadsheetDocument Document => _document.EnsureDocument();
    private WorkbookPart WbPart => _workbookPart.EnsureWorkbookPart();
    //private WorksheetPart WbSheet => 
    private string ActiveSheetName => _activeSheetName.EnsureActiveSheetName();
    private int _currentRow = 1;
    private int _currentColumn = 1;

    // 使用 MemoryStream 封裝原始流，避免直接操作檔案導致的鎖定
    private MemoryStream? _innerStream;
    private XlxStyleManager? _styleManager;

    // =============================
    //  Workbook 基本操作
    // =============================

    public IExcelEngine CreateXLSXWorkbook()
    {
        _innerStream = new MemoryStream();
        _document = SpreadsheetDocument.Create(_innerStream, SpreadsheetDocumentType.Workbook);
        _workbookPart = _document.AddWorkbookPart();
        _workbookPart.Workbook = new Workbook();
        _workbookPart.Workbook.AppendChild(new Sheets());
        AddWorksheet("Sheet1");
        return this;
    }
    public IExcelEngine CreateXLSWorkbook() => throw new NotSupportedException("OpenXML 僅支援 .xlsx 格式。");
    public IExcelEngine LoadWorkbook(string path)
    {
        _document = SpreadsheetDocument.Open(path, true);
        _workbookPart = _document.WorkbookPart;
        _activeSheetName = GetWorksheetNames().Values.FirstOrDefault();
        return this;
    }
    public IExcelEngine SaveToFile(string path)
    {
        byte[] data = SaveToByteArray();
        File.WriteAllBytes(path, data);
        return this;
    }

    public MemoryStream SaveToStream()
    {
        // 1. 確保所有異動已寫回
        WbPart.Workbook.Save();
        Document.Save();

        var output = new MemoryStream();

        // 修正 CS0131：使用傳統 if 判斷
        if (_innerStream != null)
        {
            _innerStream.Position = 0; // 現在這是安全的，因為已經確定不為 null
            _innerStream.CopyTo(output);
        }

        output.Position = 0;
        return output;
    }

    public byte[] SaveToByteArray()
    {
        // 呼叫內部流的 ToArray，這是最簡單且高效的作法
        WbPart.Workbook.Save();
        Document.Save();

        return _innerStream?.ToArray() ?? [];
    }

    // =============================
    //  Worksheet 操作
    // =============================

    public IExcelEngine AddWorksheet(string sheetName)
    {
        // 直接使用 WbPart，若為 null 會拋出 Ensure 內的錯誤訊息
        WorksheetPart newWorksheetPart = WbPart.AddNewPart<WorksheetPart>();
        newWorksheetPart.Worksheet = new Worksheet(new SheetData());

        // 確保 Sheets 容器存在，不存在則建立 (使用空合併賦值)
        Sheets sheets = WbPart.Workbook.Sheets ?? WbPart.Workbook.AppendChild(new Sheets());

        uint sheetId = (uint)(sheets.Count() + 1);
        Sheet sheet = new()
        {
            Id = WbPart.GetIdOfPart(newWorksheetPart),
            SheetId = sheetId,
            Name = sheetName
        };
        sheets.Append(sheet);

        SetActiveWorksheet(sheetName);
        return this;
    }

    public IExcelEngine SetActiveWorksheet(string sheetName)
    {
        var sheet = WbPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Name == sheetName);
        if (sheet != null)
        {
            //_activeWorksheetPart = (WorksheetPart)_workbookPart.GetPartById(sheet.Id);
            _activeSheetName = sheetName;
        }
        return this;
    }

    public string? GetActiveWorksheetName() => _activeSheetName;

    public Dictionary<int, string> GetWorksheetNames()
    {
        var dict = new Dictionary<int, string>();
        var sheets = WbPart.EnsureSheets().Cast<Sheet>().ToList();
        for (int i = 0; i < sheets?.Count; i++)
        {
            dict.Add(i, sheets[i].Name!);
        }
        return dict;
    }

    public IExcelEngine DeleteWorksheet(string sheetName)
    {
        // 1. 從 Workbook 中找到 Sheet 節點
        var targetSheet = WbPart.EnsureSheets().Cast<Sheet>().FirstOrDefault(s => s.Name == sheetName);

        if (targetSheet == null) return this;
        // 2. 移除對應的 WorksheetPart
        string relationshipId = targetSheet.EnsureId();
        WorksheetPart wsPart = (WorksheetPart)WbPart.GetPartById(relationshipId);
        WbPart.DeletePart(wsPart);

        // 3. 從 Sheets 集合中移除該節點
        targetSheet.Remove();

        // 4. 如果刪除的是當前作用中的工作表，清空狀態
        if (_activeSheetName == sheetName)
        {
            _activeSheetName = null;
        }

        // 5. 儲存 Workbook 結構變更
        WbPart.Workbook.Save();
        return this;
    }

    public IExcelEngine RenameWorksheet(string oldName, string newName)
    {
        var sheet = WbPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Name == oldName);
        if (sheet != null) sheet.Name = newName;
        return this;
    }

    public IExcelEngine HideWorksheet(string sheetName)
    {
        var sheet = WbPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Name == sheetName);
        if (sheet != null) sheet.State = SheetStateValues.Hidden;
        return this;
    }
    public IExcelEngine VeryHideWorksheet(string sheetName)
    {
        var sheet = WbPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Name == sheetName);
        if (sheet != null) sheet.State = SheetStateValues.VeryHidden;
        return this;
    }

    public IExcelEngine UnhideWorksheet(string sheetName)
    {
        var sheet = WbPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Name == sheetName);
        if (sheet != null) sheet.State = SheetStateValues.Visible;
        return this;
    }

    // =============================
    //  Row / Column 狀態控制
    // =============================

    public IExcelEngine SetActiveRow(int row) { _currentRow = row; return this; }
    public int? GetActiveRow() => _currentRow;
    public IExcelEngine SetActiveColumn(int column) { _currentColumn = column; return this; }
    public int? GetActiveColumn() => _currentColumn;
    public IExcelEngine NextRow() { _currentRow++; _currentColumn = 1; return this; }
    public IExcelEngine NextColumn() { _currentColumn++; return this; }

    // =============================
    //  Cell 操作核心 (OpenXML 特有邏輯)
    // =============================
    public IExcelEngine SetCellValue(string sheetName, int row, int column, object value)
    {
        WorksheetPart wsPart = GetWorksheetPartByName(sheetName);
        var sheetData = wsPart.Worksheet.GetFirstChild<SheetData>()
                        ?? wsPart.Worksheet.AppendChild(new SheetData());
        string cellRef = ExcelHelper.GetColumnName(column) + row;
        // 取得列
        var r = ExcelHelper.GetOrAddRow(sheetData, (uint)row);
        // 取得儲存格
        var cell = ExcelHelper.GetOrAddCell(r, cellRef);
        if (value == null) { cell.CellValue = null; return this; }

        if (decimal.TryParse(value.ToString(), out _))
        {
            cell.DataType = CellValues.Number;
            cell.CellValue = new CellValue(value.ToString()!);
        }
        else
        {
            // 進階重構建議：此處應實作 SharedStringTable 邏輯以減少體積
            cell.DataType = CellValues.InlineString;
            cell.InlineString = new InlineString(new Text(value.ToString()!));
        }
        return this;
    }
    public IExcelEngine SetCellValue(object value)
    {
        SetCellValue(ActiveSheetName, _currentRow, _currentColumn, value);
        _currentColumn++; // 自動向右遞增
        return this;
    }
    public object? GetCellValue(string sheetName, int row, int column)
    {
        var wsPart = GetWorksheetPartByName(sheetName);
        var cell = wsPart.Worksheet.Descendants<Cell>()
            .FirstOrDefault(c => c.CellReference == ExcelHelper.GetColumnName(column) + row);

        if (cell?.CellValue == null) return null;
        string value = cell.CellValue.InnerText;

        if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
        {
            return WbPart.SharedStringTablePart?.SharedStringTable.ElementAt(int.Parse(value)).InnerText;
        }
        return value;
    }
    public IExcelEngine MergeCells(string sheetName, int startRow, int startColumn, int endRow, int endColumn)
    {
        var wsPart = GetWorksheetPartByName(sheetName);
        var worksheet = wsPart.Worksheet;
        MergeCells mergeCells = worksheet.Elements<MergeCells>().FirstOrDefault() ?? new MergeCells();

        if (worksheet.Elements<MergeCells>().Any())
            worksheet.InsertAfter(mergeCells, worksheet.GetFirstChild<SheetData>());

        string cellRange = $"{ExcelHelper.GetColumnName(startColumn)}{startRow}:{ExcelHelper.GetColumnName(endColumn)}{endRow}";
        mergeCells.Append(new MergeCell() { Reference = cellRange });
        return this;
    }

    // =============================
    //  Row/Column 增刪 (最複雜的部分)
    // =============================
    public IExcelEngine DeleteRow(string sheetName, int row)
    {
        var wsPart = GetWorksheetPartByName(sheetName);
        // 使用 ?. 確保安全
        var sheetData = wsPart.Worksheet.GetFirstChild<SheetData>();
        if (sheetData == null) return this;

        // 1. 刪除目標列
        var rowsToDelete = sheetData.Elements<Row>()
            .Where(r => r.RowIndex?.Value == (uint)row).ToList();
        foreach (var r in rowsToDelete) r.Remove();

        // 2. 更新後續列的索引
        // 注意：row 傳入值需轉為 uint 進行比較
        var rowsToUpdate = sheetData.Elements<Row>().Where(r => r.RowIndex?.Value > (uint)row);
        foreach (var r in rowsToUpdate)
        {
            // 解決 CS8602: 使用 ! 告訴編譯器 RowIndex 絕對存在 (因 Where 已過濾)
            r.RowIndex!.Value--;
            foreach (Cell c in r.Elements<Cell>())
            {
                // 解決 CS8600: 使用 ?? 確保 oldRef 絕對有值，或使用 .Value
                string oldRef = c.CellReference?.Value ?? string.Empty;
                if (!string.IsNullOrEmpty(oldRef))
                {
                    // 解決 CS8604: 此時 oldRef 已保證非 null
                    int colIndex = ExcelHelper.GetColumnIndex(oldRef);
                    string colName = ExcelHelper.GetColumnName(colIndex);
                    // 更新 CellReference (注意：r.RowIndex 本身是物件，應取其 Value)
                    c.CellReference = colName + r.RowIndex.Value;
                }
            }
        }
        return this;
    }
    public IExcelEngine InsertRow(string sheetName, int rowIndex)
    {
        var wsPart = GetWorksheetPartByName(sheetName);
        var sheetData = wsPart.Worksheet.GetFirstChild<SheetData>();
        if (sheetData == null) return this;
        uint targetIndex = (uint)rowIndex;
        // 1. 找出所有索引大於或等於插入位置的 Row
        var rowsToMove = sheetData.Elements<Row>()
            .Where(r => r.RowIndex?.Value >= targetIndex)
            .OrderByDescending(r => r.RowIndex!.Value)
            .ToList();

        foreach (var row in rowsToMove)
        {
            uint oldRowIndex = row.RowIndex!.Value;
            uint newRowIndex = oldRowIndex + 1;
            row.RowIndex = newRowIndex;
            // 2. 更新該 Row 下所有 Cell 的 Reference
            foreach (Cell cell in row.Elements<Cell>())
            {
                string cellRef = cell.CellReference?.Value ?? string.Empty;
                if (StringUtils.IsNotEmpty(cellRef))
                {
                    string colName = ExcelHelper.GetColumnNameFromRef(cellRef);
                    cell.CellReference = colName + newRowIndex;
                }
            }
        }
        // 3. 插入新的空白列（確保結構完整性）
        ExcelHelper.GetOrAddRow(sheetData, (uint)rowIndex);
        return this;
    }
    public IExcelEngine InsertColumn(string sheetName, int colIndex)
    {
        var wsPart = GetWorksheetPartByName(sheetName);
        var sheetData = wsPart.Worksheet.GetFirstChild<SheetData>();
        if (sheetData == null) return this;

        // 遍歷所有 Row
        foreach (var row in sheetData.Elements<Row>())
        {
            // 1. 解決 CS8604: 先篩選掉 CellReference 為 null 的情況，並快取 Index 以提高效能
            var cellsToMove = row.Elements<Cell>()
                .Select(c => new
                {
                    Cell = c,
                    // 使用 ?.Value 並提供預設值 0 (或不處理)
                    Index = ExcelHelper.GetColumnIndexFromRef(c.CellReference?.Value ?? string.Empty)
                })
                .Where(x => x.Index >= colIndex)
                .OrderByDescending(x => x.Index)
                .ToList();

            foreach (var item in cellsToMove)
            {
                int newColIdx = (int)item.Index + 1;

                // 2. 更新 Reference (例如 B5 -> C5)
                // 解決 CS8604: 使用 row.RowIndex!.Value 確保不為空
                item.Cell.CellReference = ExcelHelper.GetColumnName(newColIdx) + row.RowIndex!.Value;
            }
        }
        return this;
    }
    public IExcelEngine DeleteColumn(string sheetName, int colIndex)
    {
        var wsPart = GetWorksheetPartByName(sheetName);
        var sheetData = wsPart.Worksheet.GetFirstChild<SheetData>();
        if (sheetData == null) return this;

        foreach (var row in sheetData.Elements<Row>())
        {
            // 1. 移除目標 Cell
            // 修正 CS8604: 使用 ?.Value ?? string.Empty
            var targetCell = row.Elements<Cell>()
                .FirstOrDefault(c => ExcelHelper.GetColumnIndexFromRef(c.CellReference?.Value ?? string.Empty) == colIndex);

            targetCell?.Remove();

            // 2. 將右側所有 Cell 左移
            // 修正 CS8604: 同樣處理 CellReference
            var cellsToMove = row.Elements<Cell>()
                .Where(c => ExcelHelper.GetColumnIndexFromRef(c.CellReference?.Value ?? string.Empty) > colIndex)
                .ToList();

            foreach (var cell in cellsToMove)
            {
                // 解決 CS8600/CS8604: 先取出字串值
                string currentRef = cell.CellReference?.Value ?? string.Empty;
                int oldColIdx = ExcelHelper.GetColumnIndexFromRef(currentRef);
                int newColIdx = oldColIdx - 1;

                // 更新 Reference
                // 修正 CS8602/CS8604: row.RowIndex!.Value 確保列號存在
                cell.CellReference = ExcelHelper.GetColumnName(newColIdx) + row.RowIndex!.Value;
            }
        }
        return this;
    }
    // =============================
    //  其餘介面實作
    // =============================
    public object GetNativeWorkbook() => Document;
    public object GetNativeWorksheet(string name) => GetWorksheetPartByName(name);
    public IExcelEngine SetCellStyle(string sheetName, int row, int column, DocumentFontStyle style)
    {
        // 取得原生的 Cell 物件 (使用之前重構的 GetNativeCell)
        Cell cell = (Cell)GetNativeCell(sheetName, row, column);
        // 透過 StyleManager 取得或建立樣式索引
        uint styleIndex = GetStyleManager().GetStyleIndex(style);
        // 應用索引
        cell.StyleIndex = styleIndex;
        return this;
    }
    public IExcelEngine SetCellStyle(int row, int column, DocumentFontStyle style) => SetCellStyle(ActiveSheetName, row, column, style);
    public IExcelEngine UnmergeCells(string sheetName, int startRow, int startColumn, int endRow, int endColumn)
    {
        var wsPart = GetWorksheetPartByName(sheetName);
        var mergeCells = wsPart.Worksheet.Elements<MergeCells>().FirstOrDefault();
        if (mergeCells != null)
        {
            string range = $"{ExcelHelper.GetColumnName(startColumn)}{startRow}:{ExcelHelper.GetColumnName(endColumn)}{endRow}";
            var target = mergeCells.Elements<MergeCell>().FirstOrDefault(m => m.Reference == range);
            target?.Remove();
            if (!mergeCells.HasChildren) mergeCells.Remove();
        }
        return this;
    }
    public IExcelEngine MoveWorksheet(string sheetName, int newIndex)
    {
        var sheets = WbPart.EnsureSheets();
        var targetSheet = sheets.Cast<Sheet>().FirstOrDefault(s => s.Name == sheetName);

        if (targetSheet != null)
        {
            targetSheet.Remove();
            var allSheets = sheets.Cast<Sheet>().ToList();

            if (newIndex >= allSheets.Count)
                sheets.AppendChild(targetSheet);
            else
                sheets.InsertAt(targetSheet, newIndex);
        }
        return this;
    }
    public IExcelEngine CopyWorksheet(string sourceSheetName, string newSheetName)
    {
        var sourcePart = GetWorksheetPartByName(sourceSheetName);
        var newPart = WbPart.AddNewPart<WorksheetPart>();

        // 複製內容
        using (var stream = sourcePart.GetStream())
        {
            newPart.FeedData(stream);
        }

        // 註冊到 Workbook
        var sheets = WbPart.EnsureSheets();
        // 1. 先處理 sheets 為空的情況，並解決 SheetId 可能為 null 的警告
        uint nextId = sheets.Cast<Sheet>()
            .Select(s => s.SheetId?.Value ?? 0) // 取出 Value，若為 null 則給 0
            .DefaultIfEmpty(0u)                // 如果集合為空，給予起始值 0
            .Max() + 1;

        Sheet newSheet = new()
        {
            Id = WbPart.GetIdOfPart(newPart),
            SheetId = nextId,
            Name = newSheetName
        };
        sheets.AppendChild(newSheet);

        return this;
    }
    public IExcelEngine DeleteWorksheets(params string[] sheetNames)
    {
        foreach (var name in sheetNames)
        {
            DeleteWorksheet(name);
        }
        return this;
    }

    /// <summary>
    /// 清除所有工作表。
    /// 為了符合 Excel 規範（至少保留一張表），我們會先建立一張臨時表，再刪除舊的所有表。
    /// </summary>
    public IExcelEngine CleanAllWorksheets()
    {
        // 1. 取得目前所有工作表名稱
        var allSheetNames = WbPart.EnsureSheets()
            .Cast<Sheet>()
            .Select(s => s.Name?.Value)
            .Where(n => n != null)
            .ToList();

        // 2. 建立一個臨時用的工作表，避免 Workbook 變為空
        string tempName = "Sheet" + DateTime.Now.Ticks;
        AddWorksheet(tempName);

        // 3. 刪除原本所有的工作表
        foreach (var name in allSheetNames)
        {
            if (name != null)
            {
                DeleteWorksheet(name);
            }
        }

        // 4. 將當前作用中工作表指向新的臨時表
        _activeSheetName = tempName;

        return this;
    }
    public IExcelEngine SetCellValue(int row, int column, object value) => SetCellValue(ActiveSheetName, row, column, value);
    public object? GetCellValue(int row, int column) => GetCellValue(ActiveSheetName, row, column);
    public IExcelEngine MergeCells(int startRow, int startColumn, int endRow, int endColumn) => MergeCells(ActiveSheetName, startRow, startColumn, endRow, endColumn);
    public IExcelEngine InsertRow(int row) => InsertRow(ActiveSheetName, row);
    public IExcelEngine DeleteRow(int row) => DeleteRow(ActiveSheetName, row);
    public IExcelEngine InsertColumn(int column) => InsertColumn(ActiveSheetName, column);
    public IExcelEngine DeleteColumn(int column) => DeleteColumn(ActiveSheetName, column);

    /// <summary>
    /// 取得原生的 Row 物件 (DocumentFormat.OpenXml.Spreadsheet.Row)。
    /// 若該列不存在則自動建立。
    /// </summary>
    public object GetNativeRow(string sheetName, int row)
    {
        // 1. 取得指定工作表的 WorksheetPart
        WorksheetPart wsPart = GetWorksheetPartByName(sheetName);

        // 2. 取得或建立 SheetData 節點 (解決 CS8600)
        // 如果 Worksheet 裡面沒有 SheetData，我們就幫它建立一個
        SheetData sheetData = wsPart.Worksheet.GetFirstChild<SheetData>()
                              ?? wsPart.Worksheet.AppendChild(new SheetData());
        // 3. 尋找對應索引的 Row (解決 CS8602，因為現在 sheetData 保證不為 null)
        Row? targetRow = sheetData.Elements<Row>()
            .FirstOrDefault(r => r.RowIndex != null && r.RowIndex.Value == (uint)row);

        // 4. 如果找到則直接回傳
        if (targetRow != null) return targetRow;

        // 5. 找不到則建立新的 Row
        targetRow = new Row { RowIndex = (uint)row };

        // 尋找插入位置 (確保 XML 順序)
        var successorRow = sheetData.Elements<Row>()
            .FirstOrDefault(r => r.RowIndex != null && r.RowIndex.Value > (uint)row);
        if (successorRow != null)
            sheetData.InsertBefore(targetRow, successorRow);
        else
            sheetData.Append(targetRow);
        return targetRow;
    }

    /// <summary>
    /// 取得原生的 Cell 物件 (DocumentFormat.OpenXml.Spreadsheet.Cell)。
    /// 若該儲存格不存在則自動建立。
    /// </summary>
    public object GetNativeCell(string sheetName, int row, int column)
    {
        // 1. 先取得原生的 Row
        Row nativeRow = (Row)GetNativeRow(sheetName, row);

        // 2. 計算 Cell Reference (例如 "A1", "B2")
        string cellReference = ExcelHelper.GetColumnName(column) + row;

        // 3. 尋找對應 Reference 的 Cell
        Cell? targetCell = nativeRow.Elements<Cell>()
            .FirstOrDefault(c => c.CellReference != null && c.CellReference == cellReference);

        // 4. 如果找不到，則建立一個新的 Cell 並加入 Row
        if (targetCell == null)
        {
            targetCell = new Cell { CellReference = cellReference };

            // 為了保持 Row 內部 Cell 的順序 (A1, B1, C1...)
            // 找到第一個 Column Index 大於當前的 Cell，並插入在其之前
            Cell? successorCell = nativeRow.Elements<Cell>()
                .FirstOrDefault(c => ExcelHelper.GetColumnIndexFromRef(c.CellReference!) > column);

            if (successorCell != null)
                nativeRow.InsertBefore(targetCell, successorCell);
            else
                nativeRow.Append(targetCell);
        }

        return targetCell;
    }

    // =============================
    //  Helper Methods (內部輔助)
    // =============================
    private WorksheetPart GetWorksheetPartByName(string name)
    {
        // 1. 取得 Sheets 集合並進行過濾
        // 使用 OfType<Sheet> 確保迭代過程中物件不為 null
        var sheet = (WbPart.EnsureSheets()
            .OfType<Sheet>()
            .FirstOrDefault(s => s.Name == name)) ?? throw new KeyNotFoundException($"找不到名為 '{name}' 的工作表。");

        // 3. 消除 CS8604：使用 EnsureId 確保傳入 GetPartById 的字串絕對不為 null
        string relId = sheet.EnsureId();
        return (WorksheetPart)WbPart.GetPartById(relId);
    }
    private XlxStyleManager GetStyleManager()
    {
        if (_styleManager == null)
        {
            var stylesPart = _workbookPart!.WorkbookStylesPart ?? _workbookPart.AddNewPart<WorkbookStylesPart>();
            _styleManager = new XlxStyleManager(stylesPart);
        }
        return _styleManager;
    }

}
