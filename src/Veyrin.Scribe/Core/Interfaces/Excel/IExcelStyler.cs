using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veyrin.Scribe.Core.Models;

namespace Veyrin.Scribe.Core.Interfaces.Excel;

public interface IExcelStyler
{
    void SetCellStyle(string sheetName, int row, int col, DocumentFontStyle style);
    void SetRangeStyle(string sheetName, int startRow, int startCol, int endRow, int endCol, DocumentFontStyle style);
    void SetBorders(string sheetName, int startRow, int startCol, int endRow, int endCol, BorderSettings settings);

    void SetColumnWidth(string sheetName, int col, double width);
    void AutoFit(string sheetName, int startCol, int endCol);
    void Freeze(string sheetName, int row, int col);
}
