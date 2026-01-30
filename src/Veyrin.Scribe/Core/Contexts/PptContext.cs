using Veyrin.Scribe.Core.Models;
namespace Veyrin.Scribe.Core.Contexts;

public class PptContext : IDocumentContext
{
    public readonly IPptEngine _engine;
    public PptContext(IPptEngine engine, DocumentOptions? options = null) => _engine = engine;
    public object GetEngine() => _engine;
}