using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;
using Veyrin.Core.Validation;
using Veyrin.Data.Infrastructure.Connections;
using Veyrin.Data.Query;

namespace Veyrin.Data.Infrastructure;

public abstract class RepositoryBase<TConnection> where TConnection : IDbConnection, new()
{
    private readonly IConfiguration _config;
    private readonly string _connName;

    protected RepositoryBase(IConfiguration config, string connectionName)
    {
        _config = config;
        _connName = connectionName;
    }

    protected IDbConnection CreateConnection() => DbConnectionProvider.Create<TConnection>(_config, _connName);

    protected IDbConnection CreateOpenConnection()
    {
        var conn = CreateConnection();
        conn.Open();
        return conn;
    }

    // ========================================================================
    // Query
    // ========================================================================
    protected T? QuerySingle<T>(string sql, object? param = null, IDbTransaction? tx = null)
    {
        if (tx != null)
            return tx.Connection!.QuerySingleOrDefault<T>(sql, param, tx);

        using var conn = CreateConnection();
        return conn.QuerySingleOrDefault<T>(sql, param);
    }

    protected IEnumerable<T> Query<T>(string sql, object? param = null, IDbTransaction? tx = null)
    {
        if (tx != null)
            return tx.Connection!.Query<T>(sql, param, tx).ToList();

        using var conn = CreateConnection();
        return conn.Query<T>(sql, param).ToList();
    }

    protected async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction? tx = null)
    {
        if (tx != null)
            return await tx.Connection!.QueryAsync<T>(sql, param, tx);

        using var conn = CreateConnection();
        return await conn.QueryAsync<T>(sql, param);
    }

    // ========================================================================
    // Execute (Insert / Update / Delete)
    // ========================================================================
    protected int Execute(string sql, object? param = null, IDbTransaction? tx = null)
    {
        if (tx != null)
            return tx.Connection!.Execute(sql, param, tx);

        using var conn = CreateConnection();
        return conn.Execute(sql, param);
    }

    protected async Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? tx = null)
    {
        if (tx != null)
            return await tx.Connection!.ExecuteAsync(sql, param, tx);

        using var conn = CreateConnection();
        return await conn.ExecuteAsync(sql, param);
    }

    // ========================================================================
    // CRUD with Condition
    // ========================================================================
    protected int Delete(string tableName, IEnumerable<Condition> conditions, IDbTransaction? tx = null)
    {
        Guard.NotEmpty(conditions, nameof(conditions));
        //var (whereSql, p) = SqlWhereBuilder.Build(where);
        var builder = new SqlWhereBuilder();
        var whereSql = builder.Add(conditions).Build();
        var p = builder.Parameters;
        return Execute($"DELETE FROM {tableName}{whereSql}", p, tx);
    }

    protected async Task<int> DeleteAsync(string tableName, IEnumerable<Condition> conditions, IDbTransaction? tx = null)
    {
        Guard.NotEmpty(conditions, nameof(conditions));
        //var (whereSql, p) = SqlWhereBuilder.Build(where);
        var builder = new SqlWhereBuilder();
        foreach (var c in conditions) builder.Add(c);
        var whereSql = builder.Build();
        var p = builder.Parameters;
        return await ExecuteAsync($"DELETE FROM {tableName}{whereSql}", p, tx);
    }

    protected int Update<T>(string tableName, T entity, IEnumerable<Condition> where, IDbTransaction? tx = null)
    {
        Guard.NotNull(entity); // 秆∕ IDE0060 材˙絋玂ウ砆ㄏノ
        Guard.NotNull(where);
        var props = typeof(T).GetProperties();
        string setSql = string.Join(", ", props.Select(p => $"{p.Name} = @{p.Name}"));
        var builder = new SqlWhereBuilder();
        // 盢 Entity 把计
        foreach (var p in props)
            builder.Parameters.Add(p.Name, p.GetValue(entity!));
        var whereSql = builder.Add(where).Build();
        return Execute($"UPDATE {tableName} SET {setSql} {whereSql}", builder.Parameters, tx);
    }

    protected async Task<int> UpdateAsync<T>(string tableName, T entity, IEnumerable<Condition> where, IDbTransaction? tx = null)
    {
        Guard.NotNull(entity);
        Guard.NotEmpty(where, message:"Update requires WHERE condition to prevent full table update");

        var props = typeof(T).GetProperties();
        string setSql = string.Join(", ", props.Select(p => $"{p.Name} = @{p.Name}"));
        var builder = new SqlWhereBuilder();
        // 盢 Entity 把计
        foreach (var p in props)
            builder.Parameters.Add(p.Name, p.GetValue(entity!));
        var whereSql = builder.Add(where).Build();
        return await ExecuteAsync($"UPDATE {tableName} SET {setSql}{whereSql}", builder.Parameters, tx);
    }

    protected int Insert<T>(string tableName, T entity, IEnumerable<string>? excludeProps = null, IDbTransaction? tx = null)
    {
        var props = typeof(T).GetProperties()
            .Where(p => excludeProps == null || !excludeProps.Contains(p.Name))
            .ToArray();

        string columns = string.Join(", ", props.Select(p => p.Name));
        string values = string.Join(", ", props.Select(p => "@" + p.Name));
        string sql = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";
        return Execute(sql, entity, tx);
    }

    protected async Task<int> InsertAsync<T>(string tableName, T entity, IEnumerable<string>? excludeProps = null, IDbTransaction? tx = null)
    {
        var props = typeof(T).GetProperties()
            .Where(p => excludeProps == null || !excludeProps.Contains(p.Name))
            .ToArray();

        string columns = string.Join(", ", props.Select(p => p.Name));
        string values = string.Join(", ", props.Select(p => "@" + p.Name));
        string sql = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";
        return await ExecuteAsync(sql, entity, tx);
    }

    protected IEnumerable<T> Query<T>(string tableName, IEnumerable<Condition>? where = null, IDbTransaction? tx = null)
    {
        Guard.NotEmpty(where, nameof(where));
        var builder = new SqlWhereBuilder();
        var whereSql = builder.Add(where!).Build();
        string sql = $"SELECT * FROM {tableName}{whereSql}";
        using var conn = CreateConnection();
        return conn.Query<T>(sql, builder.Parameters, tx);
    }

    protected T? QuerySingleOrDefault<T>(string tableName, IEnumerable<Condition>? where = null, IDbTransaction? tx = null)
    {
        Guard.NotEmpty(where, nameof(where));
        var builder = new SqlWhereBuilder();
        var whereSql = builder.Add(where!).Build();
        string sql = $"SELECT * FROM {tableName}{whereSql}";
        using var conn = CreateConnection();
        return conn.QuerySingleOrDefault<T>(sql, builder.Parameters, tx);
    }

    protected T QuerySingle<T>(string tableName, IEnumerable<Condition> where, IDbTransaction? tx = null)
    {
        Guard.NotEmpty(where, nameof(where));
        var builder = new SqlWhereBuilder();
        var whereSql = builder.Add(where).Build();
        string sql = $"SELECT * FROM {tableName}{whereSql}";
        using var conn = CreateConnection();
        return conn.QuerySingle<T>(sql, builder.Parameters, tx);
    }


}