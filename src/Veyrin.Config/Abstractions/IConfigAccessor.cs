
namespace Veyrin.Config.Abstractions;

public interface IConfigAccessor
{
    /// <summary>
    /// 取得單一設定值（支援 Enum / 基本型別）
    /// </summary>
    T Get<T>(string key, T defaultValue = default!);

    /// <summary>
    /// 取得設定區段並轉為強型別物件
    /// </summary>
    T GetSection<T>(string sectionName) where T : new();
}
