


using Veyrin.Core.Models;

public static class CollectionUtils
{
    #region List & HashSet
    public static List<T> CreateList<T>() => [];
    public static List<T> CreateList<T>(params T[] items) => [.. items];
    public static List<T> CreateList<T>(IEnumerable<T> items) => [.. items];
    public static List<T> CreateList<T>(IEnumerable<T> items, Func<T, bool> filter) => [.. items.Where(filter)];

    #endregion
    #region Set
    public static HashSet<T> CreateHashSet<T>() => [];
    public static HashSet<T> CreateHashSet<T>(params T[] items) => [.. items];
    public static HashSet<T> CreateHashSet<T>(IEnumerable<T> items) => [.. items];
    public static HashSet<T> CreateHashSet<T>(IEnumerable<T> items, Func<T, bool> filter) => [.. items.Where(filter)];
    #endregion
    #region List & HashSet Operations
    public static bool IsEmpty<T>(this IEnumerable<T>? items) => items == null || !items.Any();
    public static bool IsNotEmpty<T>(IEnumerable<T>? items) => !IsEmpty(items);
    public static T GetT<T>(IEnumerable<T>? items, int index, T defaultValue = default!)
    {
        if (items == null || !items.Any() || index < 0 || index >= items.Count()) return defaultValue;
        return items.ElementAt(index);
    }
    public static bool Contains<T>(T item, params T[] items) => CreateList(items).Contains(item);
    /// <summary>
    /// 取聯集（去重）
    /// </summary>
    public static IEnumerable<T> Union<T>(params IEnumerable<T>[] collections) => collections.SelectMany(c => c).Distinct();

    /// <summary>
    /// 取交集
    /// </summary>
    public static IEnumerable<T> Intersect<T>(params IEnumerable<T>[] collections)
    {
        if (collections == null || collections.Length == 0) return [];
        return collections.Aggregate((prev, next) => prev.Intersect(next));
    }

    /// <summary>
    /// 取差集（第一個集合扣除其他所有集合）
    /// </summary>
    public static IEnumerable<T> Except<T>(IEnumerable<T> first, params IEnumerable<T>[] others)
    {
        if (first == null) return [];

        var result = first;
        foreach (var other in others)
        {
            result = result.Except(other);
        }
        return result;
    }

    /// <summary>
    /// 合併（不去重）
    /// </summary>
    public static IEnumerable<T> Merge<T>(params IEnumerable<T>[] collections) => collections.SelectMany(static c => c);

    /// <summary>
    /// 檢查集合內是否包含 Null 元素
    /// </summary>
    public static bool ContainsNull<T>(this IEnumerable<T?>? items) where T : class
    {
        if (items == null) return false;
        return items.Any(static item => item == null);
    }

    /// <summary>
    /// 檢查集合的元素數量是否在指定範圍內
    /// </summary>
    public static bool IsCountInRange<T>(
        this IEnumerable<T>? items,
        int min,
        int max,
        RangeBoundary boundary = RangeBoundary.Inclusive)
    {
        if (items == null) return false;

        // 效能優化：優先使用已知長度的屬性
        int count = items switch
        {
            ICollection<T> c => c.Count,
            IReadOnlyCollection<T> rc => rc.Count,
            _ => items.Count()
        };

        // 複用數值擴充方法 (IsAtLeast, IsAtMost, etc.)
        return boundary switch
        {
            RangeBoundary.Inclusive => count.IsAtLeast(min) && count.IsAtMost(max),
            RangeBoundary.Exclusive => count.IsMoreThan(min) && count.IsLessThan(max),
            RangeBoundary.InclusiveMin => count.IsAtLeast(min) && count.IsLessThan(max),
            RangeBoundary.InclusiveMax => count.IsMoreThan(min) && count.IsAtMost(max),
            _ => throw new ArgumentOutOfRangeException(nameof(boundary))
        };
    }

    #endregion
    #region Dictionary
    public static Dictionary<TK, TV> CreateDictionary<TK, TV>() where TK : notnull => [];
    #endregion

    #region Extensions
    // GetFirst
    public static T? GetFirst<T>(this IReadOnlyList<T?> list) =>
        list.Count > 0 ? list[0] : default;

    // GetLast
    public static T? GetLast<T>(this IReadOnlyList<T?> list) =>
        list.Count > 0 ? list[^1] : default;

    // Get(index)
    public static T? Get<T>(this IReadOnlyList<T?> list, int index) =>
        index >= 0 && index < list.Count ? list[index] : default;

    // Get(index, defaultValue)
    public static T? Get<T>(this IReadOnlyList<T?> list, int index, T? defaultValue) =>
        index >= 0 && index < list.Count ? list[index] : defaultValue;
    #endregion
}
