namespace Veyrin.Data.Query;

public enum LikeWildcardMode
{
    None,       // 不加 %
    Left,       // 加在左邊：%value
    Right,      // 加在右邊：value%
    Both        // 左右都加：%value%
}