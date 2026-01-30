using Veyrin.Scribe.Core.Interfaces;
using Veyrin.Scribe.Core.Interfaces.Word;
using Veyrin.Scribe.Core.Models;

namespace Veyrin.Scribe.NPOI
{
    public class PptEngine : IPptEngine, IEngine
    {
        public IPptEngine AddSlide(SlideLayoutType layout)
        {
            throw new NotImplementedException();
        }

        public IPptEngine AddTextBox(float x, float y, float width, float height, string text)
        {
            throw new NotImplementedException();
        }

        public IPptEngine AddTextBox(float x, float y, float width, float height, string text, DocumentFontStyle style)
        {
            throw new NotImplementedException();
        }

        public IPptEngine CreatePresentation()
        {
            throw new NotImplementedException();
        }

        public IPptEngine DeleteSlide(int slideIndex)
        {
            throw new NotImplementedException();
        }

        public object GetNativePresentation()
        {
            throw new NotImplementedException();
        }

        public object GetNativeShape(int slideIndex, int shapeIndex)
        {
            throw new NotImplementedException();
        }

        public object GetNativeSlide(int index)
        {
            throw new NotImplementedException();
        }

        public int GetSlideCount()
        {
            throw new NotImplementedException();
        }

        public IWordEngine InsertChart(ChartType type, float x, float y, float width, float height)
        {
            throw new NotImplementedException();
        }

        public IWordEngine InsertImage(float x, float y, float width, float height, string imagePath)
        {
            throw new NotImplementedException();
        }

        public IWordEngine InsertTable(float x, float y, float width, float height, int rows, int cols)
        {
            throw new NotImplementedException();
        }

        public IPptEngine LoadPresentation(string path)
        {
            throw new NotImplementedException();
        }

        public IPptEngine MoveSlide(int oldIndex, int newIndex)
        {
            throw new NotImplementedException();
        }

        public IPptEngine ReplacePlaceholderText(string placeholderTag, string text)
        {
            throw new NotImplementedException();
        }

        public byte[] SaveToByteArray()
        {
            throw new NotImplementedException();
        }

        public IPptEngine SaveToFile(string path)
        {
            throw new NotImplementedException();
        }

        public MemoryStream SaveToStream()
        {
            throw new NotImplementedException();
        }

        public IPptEngine SetActiveSlide(int slideIndex)
        {
            throw new NotImplementedException();
        }

        public IPptEngine SetSlideNotes(string notes)
        {
            throw new NotImplementedException();
        }

        public IPptEngine SetTransition(TransitionEffect effect)
        {
            throw new NotImplementedException();
        }
    }
}