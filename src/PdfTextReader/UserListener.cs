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

        public UserListener() { }

        public UserListener(Action<Block> callback)
        {
            this._callback = callback ?? throw new ArgumentNullException(nameof(callback));
        }

        public void EventOccurred(IEventData data, EventType type)
        {

            if (data is TextRenderInfo textInfo)
            {
                var baseline = textInfo.GetBaseline().GetStartPoint();
                var descent = textInfo.GetDescentLine().GetStartPoint();
                var ascent = textInfo.GetAscentLine().GetEndPoint();

                var font = textInfo.GetFont().GetFontProgram();
                var FamilyName = font.GetFontNames().GetFamilyName();
                string FamilyNameString = String.Empty;
                if (FamilyName == null)
                {
                    FamilyNameString = font.GetFontNames().GetFontName();
                }
                else
                {
                    FamilyNameString = font.GetFontNames().GetFamilyName()[0][3];
                }

                var block = new Block()
                {
                    Text = textInfo.GetText(),
                    X = descent.Get(0),
                    H = descent.Get(1),
                    B = baseline.Get(1),
                    Width = ascent.Get(0) - descent.Get(0),
                    Height = ascent.Get(1) - descent.Get(1),
                    Lower = baseline.Get(1) - descent.Get(1),
                    FontFullName = font.GetFontNames().GetFontName(),
                    FontStyle = TrimFontStyle(font.GetFontNames().GetFontName()),
                    FontSize = textInfo.GetFontSize(),
                    WordSpacing = textInfo.GetWordSpacing()
                };
                block.FontName = FamilyNameString;

                string text = textInfo.GetText();

                _callback(block);
            }
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

        public ICollection<EventType> GetSupportedEvents()
        {
            return _supportedEvents;
        }
    }
}
