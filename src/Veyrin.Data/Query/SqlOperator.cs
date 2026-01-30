using System.ComponentModel;

namespace Veyrin.Data.Query;

public enum SqlOperator
{
    [Description("=")]
    Equal,          // =
    [Description("<>")]
    NotEqual,       // <>
    [Description(">")]
    GreaterThan,    // >
    [Description("<")]
    LessThan,       // <
    [Description(">=")]
    GreaterOrEqual, // >=
    [Description("<=")]
    LessOrEqual,    // <=
    [Description("LIKE")]
    Like,           // LIKE
    [Description("NOT LIKE")]
    NotLike,        // NOT LIKE
    [Description("IN")]
    In,             // IN
    [Description("NOT IN")]
    NotIn           // NOT IN
}