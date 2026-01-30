using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using Veyrin.Core.Validation;

namespace Veyrin.Data.Infrastructure.Connections;

public static class DbConnectionProvider
{
    public static TConnection Create<TConnection>(IServiceProvider provider, string connectionName)
        where TConnection : IDbConnection, new()
    {
        var configuration = provider.GetRequiredService<IConfiguration>();
        return Create<TConnection>(configuration, connectionName);
    }
    public static TConnection Create<TConnection>(IConfiguration cfg, string dbName)
        where TConnection : IDbConnection, new()
    {
        var cs = cfg.GetConnectionString(dbName) ?? "";
        Console.WriteLine(cs);
        Guard.Check(cs).ThrowIfInvalid();
        return new TConnection() { ConnectionString = cs };
    }
}