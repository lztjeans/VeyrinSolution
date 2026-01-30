using iText.Kernel.Pdf;
using iText.Layout;
using Veyrin.Scribe.Core.Interfaces;

namespace Veyrin.Scribe.Pdf.iText7;

public class PdfEngine : IPdfEngine, IEngine
{
    private readonly MemoryStream _ms;
    private readonly PdfDocument _pdf;
    private readonly Document _document;

    public PdfEngine()
    {
        _ms = new MemoryStream();
        _pdf = new PdfDocument(new PdfWriter(_ms));
        _document = new Document(_pdf);
    }

    public byte[] SaveToByteArray()
    {
        throw new NotImplementedException();
    }

    public MemoryStream SaveToStream()
    {
        throw new NotImplementedException();
    }
}