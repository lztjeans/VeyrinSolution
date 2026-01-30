using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veyrin.Scribe.Core.Interfaces.Excel;

public interface IExcelWorkbook
{
    void CreateXLS();
    void CreateXLSX();
    void Load(string path);
    object GetNativeWorkbook(); // 存取底層物件
}
