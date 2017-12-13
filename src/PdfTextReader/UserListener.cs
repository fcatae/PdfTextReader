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
        private readonly List<EventType> _supportedEvents = new List<EventType>() { EventType.RENDER_TEXT };
        private readonly Action<Block> _callback;

        public UserListener(Action<Block> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            this._callback = callback;
        }

        public void EventOccurred(IEventData data, EventType type)
        {
            var textInfo = data as TextRenderInfo;

            if ( textInfo != null )
            {
                var baseline = textInfo.GetBaseline().GetStartPoint();
                var descent = textInfo.GetDescentLine().GetStartPoint();
                var ascent = textInfo.GetAscentLine().GetEndPoint();

                var block = new Block()
                {
                    Text = textInfo.GetText(),
                    X = descent.Get(0),
                    H = descent.Get(1),
                    B = baseline.Get(1),
                    Width = ascent.Get(0) - descent.Get(0),
                    Height = ascent.Get(1) - descent.Get(1),
                    Lower = baseline.Get(1) - descent.Get(1)
                };

                string text = textInfo.GetText();

                _callback(block);
            }
        }

        public ICollection<EventType> GetSupportedEvents()
        {
            return _supportedEvents;
        }
    }
}
