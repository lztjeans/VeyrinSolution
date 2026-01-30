namespace Veyrin.Core.Html;
public class DataTableElement : AbstractElements
{
    public List<string> ColumnHeaders { get; set; } = [];

    public static DataTableElement Create(string id, params string[] ths)
    {
        return new DataTableElement
        {
            Id = $"tbl{id}",
            ColumnHeaders = [.. ths],
            IsRequired = false,
            //ReadOnly = false
        };
    }
}