namespace Veyrin.Data.Mapping;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class TableNameAttribute : Attribute
{
    public string SchemaSrc { get; } = string.Empty;
    public string SchemaTar { get; } = string.Empty;
    public string Name { get; }
    public TableNameAttribute(string tblName, string srcSchema, string tarSchema)
    {
        Name = tblName;
        SchemaSrc = srcSchema;
        SchemaTar = tarSchema;
    }
}