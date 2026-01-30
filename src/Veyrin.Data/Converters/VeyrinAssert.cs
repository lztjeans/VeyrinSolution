using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veyrin.Core.Exceptions;

namespace Veyrin.Data.Converters
{
    public static class VeyrinAssert
    {
        // 專門測試 DataTable 轉換是否正確
        public static void ShouldMatchSchema<T>(DataTable table)
        {
            var properties = typeof(T).GetProperties();
            foreach (var prop in properties)
            {
                if (!table.Columns.Contains(prop.Name))
                    throw new AssertException($"表格缺少屬性: {prop.Name}");
            }
        }
    }
}
