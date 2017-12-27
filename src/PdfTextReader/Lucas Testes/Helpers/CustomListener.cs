using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Lucas_Testes.Helpers
{
    public class CustomListener : IEventListener
    {
        double PageHeight;

        private readonly List<EventType> _supportedEvents = new List<EventType>() { EventType.RENDER_TEXT, EventType.RENDER_IMAGE };

        List<MainItem> items = new List<MainItem>();
        List<Block> blocks = new List<Block>();

        public CustomListener(float top)
        {
            this.PageHeight = top;
        }

        public void BeginTextBlock() { }

        public void EventOccurred(IEventData data, EventType type)
        {
            if (items.Count == 1)
            {
                var item = items[0];
                if (item.GetType() ==  typeof(ImageItem))
                {
                    PageHeight = item.GetLL().GetY();
                }
            }
            if (data is TextRenderInfo textInfo)
            {
                RenderText(textInfo);
            }
            if (data is ImageRenderInfo imageInfo)
            {
                RenderImage(imageInfo);
            }
        }

        public ICollection<EventType> GetSupportedEvents()
        {
            return _supportedEvents;
        }

        public void RenderText(TextRenderInfo textRenderInfo)
        {
            items.Add(new TextItem(textRenderInfo, PageHeight));

            //Block
            var baseline = textRenderInfo.GetBaseline().GetStartPoint();
            var descent = textRenderInfo.GetDescentLine().GetStartPoint();
            var ascent = textRenderInfo.GetAscentLine().GetEndPoint();

            var font = textRenderInfo.GetFont().GetFontProgram();
            var FamilyName = font.GetFontNames().GetFamilyName();

            var block = new Block()
            {
                Text = textRenderInfo.GetText(),
                X = descent.Get(0),
                H = descent.Get(1),
                B = baseline.Get(1),
                Width = ascent.Get(0) - descent.Get(0),
                Height = ascent.Get(1) - descent.Get(1),
                Lower = baseline.Get(1) - descent.Get(1),
                FontFullName = font.GetFontNames().GetFontName(),
                FontName = FamilyName[0][3],
                FontStyle = TrimFontStyle(font.GetFontNames().GetFontName()),
                FontSize = textRenderInfo.GetFontSize(),
                WordSpacing = textRenderInfo.GetWordSpacing()
            };
        }
        string TrimFontStyle(String name)
        {
            if (name == null)
            {
                return null;
            }
            if (name.EndsWith("Bold"))
            {
                return "Bold";
            }
            else if (name.EndsWith("Italic"))
            {
                return "Italic";
            }
            else if (name.EndsWith("BoldItalic"))
            {
                return "BoldItalic";
            }
            else
            {
                return "Regular";
            }
        }

        public void RenderImage(ImageRenderInfo imageRenderInfo)
        {
            items.Add(new ImageItem(imageRenderInfo));
        }

        public void EndTextBlock() { }

        public List<MainItem> GetItems()
        {
            return items;
        }
    }
}
