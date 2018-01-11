using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using PdfTextReader.Execution;
using System;
using System.Collections.Generic;
using System.Text;
using PdfTextReader.Base;
using iText.IO.Font;

namespace PdfTextReader.PDFText
{
    class ProcessPdfText : IEventListener, IPipelineResults<BlockPage>
    {
        private readonly List<EventType> _supportedEvents = new List<EventType>() { EventType.RENDER_TEXT };

        private BlockSet<Block> _blockSet = new BlockSet<Block>();

        public void EventOccurred(IEventData data, EventType type)
        {
            if (data is TextRenderInfo textInfo)
            {
                var baseline = textInfo.GetBaseline().GetStartPoint();
                var descent = textInfo.GetDescentLine().GetStartPoint();
                var ascent = textInfo.GetAscentLine().GetEndPoint();
                var font = textInfo.GetFont().GetFontProgram().GetFontNames();
                var fontName = GetFontName(font);
                var fontStyle = GetFontStyle(font);

                // calculate font-size
                float ctm_y = textInfo.GetTextMatrix().Get(4);
                float fontSize = textInfo.GetFontSize() * ctm_y;

                var block = new Block()
                {
                    Text = textInfo.GetText(),
                    X = descent.Get(0),
                    H = descent.Get(1),
                    B = baseline.Get(1),
                    Width = ascent.Get(0) - descent.Get(0),
                    Height = ascent.Get(1) - descent.Get(1),
                    Lower = baseline.Get(1) - descent.Get(1),
                    FontFullName = font.GetFontName(),
                    FontName = fontName,
                    FontStyle = fontStyle,
                    FontSize = fontSize,
                    IsBold = (fontStyle == "Bold"),
                    IsItalic = (fontStyle == "Italic"),
                    WordSpacing = textInfo.GetWordSpacing()
                };

                // valuable log information: sometimes FontStyle is wrong!
                // var workfont = WorkFont(font);
                                
                float dbgWordSpacing = textInfo.GetWordSpacing();
                
                if(dbgWordSpacing==0)
                {
                    // why 0?
                }

                _blockSet.Add(block);
            }
        }
        
        object WorkFont(FontNames font)
        {
            var stat = new
            {
                Name = font.GetFontName(),
                Weight = font.GetFontWeight(),
                IsBold = font.IsBold(),
                GetFamilyName = font.GetFamilyName()
            };

            return stat;
        }

        string GetFontName(FontNames font)
        {
            // Family Name - sometimes return null
            string[][] familyNameArray = font.GetFamilyName();

            string fontname = font.GetFontName();

            return RemoveSubFontPrefix(fontname);
        }
        
        string RemoveSubFontPrefix(string name)
        {
            // According to PDF specification ISO 32000-1:2008, 
            // font subsets names are prefixed with 6 uppercase letters followed by plus (+) sign.
            // ref https://developers.itextpdf.com/question/what-are-extra-characters-font-name-my-pdf

            if (name.IndexOf("+") == 6)
                return name.Substring(6);

            return name;
        }

        bool IsFontBold(FontNames font)
        {
            const int normalWeight = 400;

            bool heavyWeight = (font.GetFontWeight() > normalWeight);
            bool itextProperty = (font.IsBold());

            return (heavyWeight || itextProperty);
        }
        
        string GetFontStyle(FontNames font)
        {
            string fontname = font.GetFontName();
            string style = "Regular";

            if (fontname.Contains("Bold"))
            {
                return "Bold";
            }
            else if (fontname.Contains("Italic"))
            {
                return "Italic";
            }
            
            // bold may apply to inline text used for subscript texts
            if (style != "Bold" && IsFontBold(font))
            {
                Console.WriteLine($"Font-style {fontname} may be bold");
            }

            return style;
        }

        public ICollection<EventType> GetSupportedEvents()
        {
            return _supportedEvents;
        }

        public BlockPage GetResults()
        {
            var blockPage = new BlockPage();

            blockPage.AddRange(_blockSet);

            return blockPage;
        }
    }
}
