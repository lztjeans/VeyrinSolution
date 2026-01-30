namespace Veyrin.Data.Query;

public class SqlWhereBuilder
{
    private readonly List<string> _conditions = [];
    public IDictionary<string, object?> Parameters { get; } = new Dictionary<string, object?>();

    #region Fluent API 入口

    public SqlWhereBuilder Add(IEnumerable<Condition> conditions)
    {
        foreach (Condition condition in conditions) Add(condition);
        return this;
    }

    /// <summary>
    /// 最通用的入口：將自定義的 Condition 物件轉換為 SQL 片段
    /// </summary>
    public SqlWhereBuilder Add(Condition? condition)
    {
        if (condition == null) return this;

        // 1. 處理 Null 邏輯 (IS NULL / IS NOT NULL)
        if (condition.Value == null)
        {
            HandleNullCondition(condition);
            return this;
        }

        // 2. 處理集合邏輯 (IN / NOT IN)
        if (condition.Operator is SqlOperator.In or SqlOperator.NotIn)
        {
            HandleInCondition(condition);
            return this;
        }

        // 3. 處理一般數值/字串/日期邏輯
        string opSql = condition.Operator.GetDescription();
        object finalValue = condition.Value;

        // 針對 Like 運算子處理萬用字元
        if (condition.Operator is SqlOperator.Like or SqlOperator.NotLike)
        {
            finalValue = FormatLikeValue(condition.Value.ToString()!, condition.LikeWildcard);
        }

        // 處理日期與欄位名稱
        string columnExpr = condition.IsDate ? $"CAST([{condition.Column}] AS DATE)" : $"[{condition.Column}]";

        return AddRawCondition(columnExpr, opSql, finalValue);
    }

    /// <summary>
    /// 快捷入口：手動傳入參數 (內部轉換為 Condition 處理)
    /// </summary>
    public SqlWhereBuilder AddStringCondition(string columnName, string? value, SqlOperator op = SqlOperator.Equal, LikeWildcardMode like = LikeWildcardMode.None)
        => Add(Condition.StringCondition(columnName, value?.ToString(), op, like));

    public SqlWhereBuilder AddNumericCondition<T>(string columnName, T value, SqlOperator op = SqlOperator.Equal) where T : struct, System.Numerics.INumber<T>
        => Add(Condition.NumericCondition<T>(columnName, value, op));

    /// <summary>
    /// 快捷入口：日期檢查
    /// </summary>
    public SqlWhereBuilder AddDateCondition(string columnName, DateTime? value, SqlOperator op = SqlOperator.Equal)
        => Add(Condition.DateTimeCondition(columnName, value, op));

    public SqlWhereBuilder AddInCondition<T>(string columnName, params T[] values) where T : struct
        => Add(Condition.InCondition(columnName, values));

    #endregion

    #region 私有邏輯處理 (Engine)

    /// <summary>
    /// 核心：產生參數化名稱並加入集合
    /// </summary>
    private SqlWhereBuilder AddRawCondition(string columnExpr, string opSql, object value)
    {
        string paramName = $"@p{Parameters.Count}";
        _conditions.Add($"{columnExpr} {opSql} {paramName}");
        Parameters.Add(paramName, value);
        return this;
    }

    private void HandleNullCondition(Condition condition)
    {
        string sql = condition.Operator switch
        {
            SqlOperator.NotEqual or SqlOperator.NotLike => $"[{condition.Column}] IS NOT NULL",
            _ => $"[{condition.Column}] IS NULL"
        };
        _conditions.Add(sql);
    }

    private void HandleInCondition(Condition condition)
    {
        //if (condition.Value is not System.Collections.IEnumerable list) return;
        if (condition.Value is not System.Collections.IEnumerable list || condition.Value is string)
            return;

        var items = list.Cast<object>().ToList();
        if(items.Count == 0) return;

        var paramNames = new List<string>();
        foreach (var item in items)
        {
            string pName = $"@p{Parameters.Count}";
            paramNames.Add(pName);
            Parameters.Add(pName, item);
        }

        if (paramNames.Count > 0)
        {
            string op = condition.Operator.GetDescription();
            _conditions.Add($"[{condition.Column}] {op} ({paramNames.Concat(", ")})");
        }
    }

    private static string FormatLikeValue(string value, LikeWildcardMode mode) => mode switch
    {
        LikeWildcardMode.Both => $"%{value}%",
        LikeWildcardMode.Left => $"%{value}",
        LikeWildcardMode.Right => $"{value}%",
        _ => value
    };

    #endregion

    public string Build() => _conditions.Count > 0
        ? $" WHERE {_conditions.Concat(" AND ")}"
        : string.Empty;
}


/*
    //private string GetOperatorSql(SqlOperator op) => op switch
    //{
    //    SqlOperator.Equal => "=",
    //    SqlOperator.NotEqual => "<>",
    //    SqlOperator.GreaterThan => ">",
    //    SqlOperator.LessThan => "<",
    //    SqlOperator.GreaterOrEqual => ">=",
    //    SqlOperator.LessOrEqual => "<=",
    //    SqlOperator.Like => "LIKE",
    //    SqlOperator.NotLike => "NOT LIKE",
    //    _ => "="
    //};
public static (string whereSql, DynamicParameters parameters) Build(IEnumerable<Condition> conditions)
{
    var clauses = new List<string>();
    var parameters = new DynamicParameters();
    int index = 0;

    foreach (var cond in conditions)
    {
        string paramName = $"@p{index}";
        string op = cond.Operator switch
        {
            SqlOperator.Equal => "=",
            SqlOperator.NotEqual => "<>",
            SqlOperator.GreaterThan => ">",
            SqlOperator.LessThan => "<",
            SqlOperator.GreaterOrEqual => ">=",
            SqlOperator.LessOrEqual => "<=",
            SqlOperator.Like => "LIKE",
            SqlOperator.NotLike => "NOT LIKE",
            SqlOperator.In => "IN",
            SqlOperator.NotIn => "NOT IN",
            _ => throw new NotSupportedException($"Unsupported operator {cond.Operator}")
        };

        string colExpr = cond.IsDate ? $"CAST({cond.Column} AS DATE)" : cond.Column;
        string clause;

        switch (cond.Operator)
        {
            case SqlOperator.Like:
            case SqlOperator.NotLike:
                string val = cond.Value?.ToString() ?? "";
                val = cond.LikeWildcard switch
                {
                    LikeWildcardMode.Left => "%" + val,
                    LikeWildcardMode.Right => val + "%",
                    LikeWildcardMode.Both => "%" + val + "%",
                    _ => val
                };
                parameters.Add(paramName, val);
                clause = $"{colExpr} {op} {paramName}";
                break;

            case SqlOperator.In:
            case SqlOperator.NotIn:
                parameters.Add(paramName, cond.Value);
                clause = $"{colExpr} {op} {paramName}";
                break;

            default:
                parameters.Add(paramName, cond.Value);
                clause = $"{colExpr} {op} {paramName}";
                break;
        }

        clauses.Add(clause);
        index++;
    }

    string whereSql = clauses.Any() ? " WHERE " + string.Join(" AND ", clauses) : "";
    return (whereSql, parameters);
}
*/