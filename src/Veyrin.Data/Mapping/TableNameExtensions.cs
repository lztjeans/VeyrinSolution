namespace Veyrin.Data.Mapping;

public static class TableNameExtensions
{
    private enum TableNameType
    {
        Target,
        Source
    }

    public static string GetTargetTableName(this Type type) => GetTableName(type, TableNameType.Target);
    public static string GetSourceTableName(this Type type) => GetTableName(type, TableNameType.Source);
    private static string GetTableName(Type type, TableNameType nameType)
    {
        var attr = type.GetCustomAttributes(typeof(TableNameAttribute), false)
                       .Cast<TableNameAttribute>()
                       .FirstOrDefault();

        return attr == null
            ? throw new InvalidOperationException($"Type {type.Name} does not define [TableName].")
            : nameType switch
            {
                TableNameType.Target => $"{attr.SchemaTar}.{attr.Name}",
                TableNameType.Source => $"{attr.SchemaSrc}.{attr.Name}",
                _ => throw new ArgumentOutOfRangeException(nameof(nameType), "Unknown TableNameType")
            };
    }

}