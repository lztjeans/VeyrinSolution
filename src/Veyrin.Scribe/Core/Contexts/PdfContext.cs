using Veyrin.Scribe.Core.Models;
namespace Veyrin.Scribe.Core.Contexts;
public class PdfContext : IDocumentContext
{
    private IPdfEngine _engine;

    public PdfContext(IPdfEngine engine, DocumentOptions? options = null) => _engine = engine;

    public object GetEngine() => _engine;
}