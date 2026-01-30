namespace Veyrin.Data.Query;

public class Condition
{
    public string Column { get; init; } = null!;
    public SqlOperator Operator { get; init; }
    public object? Value { get; init; }
    public LikeWildcardMode LikeWildcard { get; init; }
    public bool IsDate => Value is DateTime or DateOnly or DateTimeOffset;

    private Condition(string column, object? _value, SqlOperator _operator, LikeWildcardMode likeWildcard)
    {
        Column = column;
        Operator = _operator;
        Value = _value;
        LikeWildcard = likeWildcard;
    }

    public static Condition DateTimeCondition(string _column, DateTime? _value, SqlOperator _operator = SqlOperator.Equal) => new(_column, _value, _operator, likeWildcard: LikeWildcardMode.None);
    public static Condition StringCondition(string _column, string? _value, SqlOperator _operator = SqlOperator.Equal, LikeWildcardMode like = LikeWildcardMode.None)
    {
        return _operator switch
        {
            SqlOperator.Like or SqlOperator.NotLike => new Condition(_column, _value, _operator, like),
            _ => new Condition(_column, _value, _operator, LikeWildcardMode.None),
        };
    }
    //public static Condition NumericCondition<T>(string _column, T? _value, SqlOperator _operator = SqlOperator.Equal)
    //where T : struct
    //{
    //    if (_value == null)
    //        return new Condition(_column, _value, _operator, LikeWildcardMode.None);

    //    Type type = typeof(T);

    //    if (type == typeof(int) ||
    //        type == typeof(double) ||
    //        type == typeof(decimal) ||
    //        type == typeof(long) ||
    //        type == typeof(float))
    //    {
    //        // 實際建構 Condition 的邏輯
    //        return new Condition(_column, _value, _operator, LikeWildcardMode.None);
    //    }

    //    throw new ArgumentException($"Unsupported numeric type: {type.Name}");
    //}
    public static Condition NumericCondition<T>(string _column, T? _value, SqlOperator _operator = SqlOperator.Equal)
    where T : struct, System.Numerics.INumber<T>
    {
        return new Condition(_column, _value, _operator, LikeWildcardMode.None);
    }
    public static Condition InCondition<T>(string _column, IEnumerable<T> _value) => new(_column, _value, SqlOperator.In, LikeWildcardMode.None);
}