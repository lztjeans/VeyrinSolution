using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veyrin.Demo.SyncTool
{
    // 1. 定義實體模型 (POCO)
    public class ProductEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
