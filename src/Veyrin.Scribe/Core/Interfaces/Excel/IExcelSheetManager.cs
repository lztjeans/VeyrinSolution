using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veyrin.Scribe.Core.Interfaces.Excel
{
    public interface IExcelSheetManager
    {
        IExcelSheetManager Add(string name);
        IExcelSheetManager Remove(string name);
        IExcelSheetManager Rename(string oldName, string newName);
        IExcelSheetManager SetActive(string name);
        string? GetActiveName();
        Dictionary<int, string> GetAllNames();
    }
}
