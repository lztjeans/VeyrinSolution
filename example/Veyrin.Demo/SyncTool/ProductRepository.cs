using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veyrin.Data.Converters;
using Veyrin.Data.Infrastructure;

namespace Veyrin.Demo.SyncTool
{
    // 2. 實作 Repository (使用我們建構的 SqlServer 基底)
    public class ProductRepository : SqlServerRepositoryBase<ProductEntity>
    {
        public ProductRepository(IConfiguration config) : base(config, "InventoryDb") { }

        public async Task SyncProductsAsync(List<ProductEntity> products)
        {
            // 建立交易確保資料完整性
            using var connection = CreateConnection();
            using var tx =(SqlTransaction) connection.BeginTransaction();

            // 使用 Converters 將 List 轉為 DataTable 並執行 BulkInsert
            var table = products.ToDataTable();
            await BulkInsertAsync(table, tx);

            tx.Commit();
        }
    }
}
