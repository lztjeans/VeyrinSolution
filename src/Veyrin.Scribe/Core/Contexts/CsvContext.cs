using Veyrin.Scribe.Core.Models;
namespace Veyrin.Scribe.Core.Contexts;
public class CsvContext : IDocumentContext
{
    private ICsvEngine _engine;

    public CsvContext(ICsvEngine engine, DocumentOptions? options = null) => _engine = engine;

    public void CreateFile() => _engine.CreateCsv();

    public object GetEngine() => _engine;

    public void LoadFile(string path) => _engine.LoadCsv(path);

    public T Read<T>() where T : class => _engine.Read<T>();

    public string ReadText() => _engine.ReadText();

    public void Write<T>(T content) where T : class => _engine.Write(content);

    public void WriteText(string content) => _engine.WriteText(content);

    public byte[] SaveToBytes() => _engine.SaveToByteArray();

    public void SaveToFile(string path) => _engine.SaveToFile(path);

    public MemoryStream SaveToStream() => _engine.SaveToStream();


}