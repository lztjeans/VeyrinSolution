using System.Collections.Concurrent;
using System.Reflection;

namespace Veyrin.Core.Reflection;

public static class ReflectionCache
{
    // 存放 Property 資訊：Type -> PropertyInfo[]
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _properties = new();

    // 存放 Attribute 資訊：MemberInfo -> Attributes[]
    private static readonly ConcurrentDictionary<MemberInfo, object[]> _attributes = new();

    public static PropertyInfo[] GetProperties(Type type) =>
        _properties.GetOrAdd(type, t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance));

    public static T[] GetAttributes<T>(MemberInfo member) where T : Attribute =>
        (T[])_attributes.GetOrAdd(member, m => m.GetCustomAttributes(typeof(T), true));
}
