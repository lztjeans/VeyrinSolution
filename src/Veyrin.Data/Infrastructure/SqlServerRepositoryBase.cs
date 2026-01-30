using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using Veyrin.Core.Exceptions;
using Veyrin.Core.Validation;
using Veyrin.Data.Mapping;
using Veyrin.Data.Query;

namespace Veyrin.Data.Infrastructure;

public abstract class SqlServerRepositoryBase<T> : RepositoryBase<SqlConnection> where T : class
{
    protected readonly string SourceTableName;
    protected readonly string TargeTableName;

    protected SqlServerRepositoryBase(IConfiguration config, string connectionName)
        : base(config, connectionName)
    {
        // 自動從類別標籤或名稱取得資料表名稱
        SourceTableName = typeof(T).GetSourceTableName();
        TargeTableName = typeof(T).GetTargetTableName();
    }

    #region 高性能大量寫入 (Bulk Insert)

    /// <summary>
    /// 使用 SqlBulkCopy 進行大量寫入
    /// </summary>
    protected async Task BulkInsertAsync(
        DataTable table,
        SqlTransaction tx,
        int batchSize = 1000,
        int timeout = 60)
    {
        Guard.NotNull(tx, nameof(tx));
        Guard.NotNull(tx.Connection, "tx.Connection");

        using var bulk = new SqlBulkCopy(
            tx.Connection,
            SqlBulkCopyOptions.Default,
            tx)
        {
            DestinationTableName = this.TargeTableName, // 使用自動映射的名稱
            BatchSize = batchSize,
            BulkCopyTimeout = timeout
        };

        // 自動建立 Column Mapping (假設 DataTable 欄位與資料庫一致)
        foreach (DataColumn column in table.Columns)
        {
            bulk.ColumnMappings.Add(column.ColumnName, column.ColumnName);
        }

        await bulk.WriteToServerAsync(table);
    }

    #endregion

    #region 自動化查詢 (整合 SqlWhereBuilder)

    /// <summary>
    /// 根據一組 Condition 取得資料
    /// </summary>
    protected string BuildSelectSqlAsync(SqlWhereBuilder builder, string columns = "*")
    {
        Guard.NotNull(builder);
        // 這裡回傳組好的 SQL，實際執行可配合 Dapper 或 DataAccessBase
        return $"SELECT {columns} FROM {SourceTableName}{builder.Build()}";
    }

    /// <summary>
    /// 刪除特定條件的資料
    /// </summary>
    protected string BuildDeleteSqlAsync(SqlWhereBuilder builder)
    {
        Guard.NotNull(builder);
        string whereClause = builder.Build();
        Guard.IsTrue((StringUtils.IsEmpty(whereClause)), "為了安全起見，不允許執行無條件的 Delete 操作。");
        return $"DELETE FROM {TargeTableName}{whereClause}";
    }

    protected string BuildUpdateSqlAsync(string setValues, SqlWhereBuilder builder)
    {
        Guard.NotEmpty(setValues);
        Guard.NotNull(builder);
        string whereClause = builder.Build();
        Guard.NotEmpty(whereClause, nameof(builder), message: $"{TargeTableName} 的更新操作必須包含 WHERE 條件以防止全表更新。");

        return $"UPDATE {TargeTableName} SET {setValues}{whereClause}";
    }

    #endregion
}