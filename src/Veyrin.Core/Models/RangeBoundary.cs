
namespace Veyrin.Core.Models;

/// <summary>
/// 定義範圍檢查時的邊界包含行為
/// </summary>
public enum RangeBoundary
{
    /// <summary> [min, max] 包含最小值與最大值 </summary>
    Inclusive,
    /// <summary> (min, max) 不含最小值與最大值 </summary>
    Exclusive,
    /// <summary> [min, max) 包含最小但不含最大 </summary>
    InclusiveMin,
    /// <summary> (min, max] 不含最小但包含最大 </summary>
    InclusiveMax
}
