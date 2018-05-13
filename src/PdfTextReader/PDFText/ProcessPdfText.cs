using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using PdfTextReader.Execution;
using System;
using System.Collections.Generic;
using System.Text;
using PdfTextReader.Base;
using iText.IO.Font;
using System.Linq;

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

                var ctm = textInfo.GetTextMatrix();

                if ((ctm.Get(1) != 0) && (ctm.Get(3) != 0))
                {
                    // sometimes ctm.Get(3) != 0, and it is normal. eg, "ISSN" -- not sure why
                    PdfReaderException.Warning("ProcessPdfText: ensure we have a simple transformation matrix (CTM)");
                    return;
                }

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

                if( ShouldRecalculateSpaces(block.Text) )
                {
                    string newtext = RecalculateSpaces(textInfo);
                    if( newtext != block.Text )
                    {
                        block.Text = newtext;
                    }
                }
                
                if (block.Width <= 0 || block.Height <= 0)
                    PdfReaderException.AlwaysThrow("block.Width <= 0 || block.Height <= 0", new IBlock[] { block });

                if (block.FontSize <= 0)
                    PdfReaderException.AlwaysThrow("block.FontSize <= 0");

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

        bool ShouldRecalculateSpaces(string text)
        {
            const char SPACE = ' ';

            if(text.Length > 2)
            {
                if (text[0] != SPACE && text[1] != SPACE)
                    return false;
            }

            int spaces = text.Count(t => t == SPACE);

            if ((text.Length > 4) && (2*spaces >= text.Length - 4))
                return true;

            return false;
        }

        string RecalculateSpaces(TextRenderInfo textInfo)
        {
            var chars = textInfo.GetCharacterRenderInfos();
            float charSize = (textInfo.GetUnscaledWidth() / chars.Count)/2.0f;
            StringBuilder sb = new StringBuilder();

            foreach (var ch in chars)
            {
                string chText = ch.GetText();

                if (chText != " " || (ch.GetUnscaledWidth() > charSize))
                    sb.Append(chText);
            }

            return sb.ToString();
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

            return RemoveFontStyleSuffix(RemoveSubFontPrefix(fontname));
        }

        string RemoveFontStyleSuffix(string name)
        {
            return name.Replace("Bold", "").Replace("Italic", "").Replace("-", "");
        }

        string RemoveSubFontPrefix(string name)
        {
            // According to PDF specification ISO 32000-1:2008, 
            // font subsets names are prefixed with 6 uppercase letters followed by plus (+) sign.
            // ref https://developers.itextpdf.com/question/what-are-extra-characters-font-name-my-pdf

            if (name.IndexOf("+") == 6)
                return name.Substring(7);

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
                PdfReaderException.Warning($"Font-style {fontname} may be bold");
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
