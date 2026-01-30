using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veyrin.Scribe.Core.Interfaces;
using Veyrin.Scribe.Core.Interfaces.Excel;

namespace Veyrin.Scribe.Excel.ClosedXML
{
    /*
     
     IExcelWorkbook,
     */

    public class ExcelEngineV2 : IEngine, IExcelEngineV2
    {
        public int ActiveRow { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int ActiveColumn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IExcelSheetManager Add(string name)
        {
            throw new NotImplementedException();
        }

        public void CreateXLS()
        {
            throw new NotImplementedException();
        }

        public void CreateXLSX()
        {
            throw new NotImplementedException();
        }

        public Tuple<int, int>? Find(string sheetName, string text)
        {
            throw new NotImplementedException();
        }

        public string? GetActiveName()
        {
            throw new NotImplementedException();
        }

        public Dictionary<int, string> GetAllNames()
        {
            throw new NotImplementedException();
        }

        public int GetLastRowIndex(string sheetName)
        {
            throw new NotImplementedException();
        }

        public object GetNativeWorkbook()
        {
            throw new NotImplementedException();
        }

        public object? GetValue(string sheetName, int row, int col)
        {
            throw new NotImplementedException();
        }

        public void ImportDataTable(string sheetName, int startRow, int startCol, DataTable dt, bool headers)
        {
            throw new NotImplementedException();
        }

        public void InsertImage(string sheetName, byte[] data, int row, int col, int w, int h)
        {
            throw new NotImplementedException();
        }

        public void Load(string path)
        {
            throw new NotImplementedException();
        }

        public IExcelSheetManager Remove(string name)
        {
            throw new NotImplementedException();
        }

        public IExcelSheetManager Rename(string oldName, string newName)
        {
            throw new NotImplementedException();
        }

        public byte[] SaveToByteArray()
        {
            throw new NotImplementedException();
        }

        public MemoryStream SaveToStream()
        {
            throw new NotImplementedException();
        }

        public IExcelSheetManager SetActive(string name)
        {
            throw new NotImplementedException();
        }

        public void SetFormula(string sheetName, int row, int col, string formula)
        {
            throw new NotImplementedException();
        }

        public void SetValue(string sheetName, int row, int col, object value)
        {
            throw new NotImplementedException();
        }
    }
}
