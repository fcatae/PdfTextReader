using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System;
using System.Collections.Generic;
using System.Text;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;

namespace PdfTextReader
{
    class UserListener : IEventListener
    {
        readonly List<EventType> _supportedEvents = new List<EventType>() { EventType.RENDER_TEXT };

        public void EventOccurred(IEventData data, EventType type)
        {
            var textInfo = data as TextRenderInfo;

            if ( textInfo != null )
            {
                string text = textInfo.GetText();
                System.Diagnostics.Debug.WriteLine(text);
            }
        }

        public ICollection<EventType> GetSupportedEvents()
        {
            return _supportedEvents;
        }
    }
}
