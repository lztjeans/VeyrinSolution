namespace Veyrin.Scribe.Core.Interfaces;

public interface IEngine
{
    /// <summary>輸出成 MemoryStream。</summary>
    MemoryStream SaveToStream();
    /// <summary>輸出成 byte 陣列。</summary>
    byte[] SaveToByteArray();
}