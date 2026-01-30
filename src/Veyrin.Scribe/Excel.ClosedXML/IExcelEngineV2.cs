using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veyrin.Scribe.Core.Interfaces.Excel;

namespace Veyrin.Scribe.Excel.ClosedXML;

public interface IExcelEngineV2 : IExcelWorkbook, IExcelSheetManager , IExcelContentEditor//, IExcelStyler
{
    // 這裡可以放「狀態型」的屬性
    int ActiveRow { get; set; }
    int ActiveColumn { get; set; }
}
