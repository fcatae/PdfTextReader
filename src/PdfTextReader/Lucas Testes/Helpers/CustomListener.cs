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
        float top;

        private readonly List<EventType> _supportedEvents = new List<EventType>() { EventType.RENDER_TEXT };

        List<MainItem> items = new List<MainItem>();

        public CustomListener(float top)
        {
            this.top = top;
        }

        public void BeginTextBlock() { }

        public void EventOccurred(IEventData data, EventType type)
        {

            if (data is TextRenderInfo textInfo)
            {
                RenderText(textInfo);
            }
        }

        public ICollection<EventType> GetSupportedEvents()
        {
            return _supportedEvents;
        }

        public void RenderText(TextRenderInfo textRenderInfo)
        {
            items.Add(new TextItem(textRenderInfo, top));
        }

        public void EndTextBlock() { }

        public List<MainItem> GetItems()
        {
            return items;
        }
    }
}
