using iText.Kernel.Pdf.Canvas.Parser.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Lucas_Testes.Helpers
{
    public class TextStyle
    {
        public string fontName;
        public float fontSize;

        public TextStyle(string fontName, float fontSize)
        {
            this.fontName = fontName;
            this.fontSize = fontSize;
        }

        public TextStyle(TextRenderInfo textRenderInfo)
        {
            string font = textRenderInfo.GetFont().GetFontProgram().GetFontNames().GetFullName()[0][3];
            if (font.Contains("+"))
                font = font.Substring(font.IndexOf("+") + 1, font.Length - font.IndexOf("+") - 1);
            if (font.Contains("-"))
                font = font.Substring(0, font.IndexOf("-"));
            this.fontName = font;
            this.fontSize = textRenderInfo.GetAscentLine().GetStartPoint().Get(1) - textRenderInfo.GetDescentLine().GetStartPoint().Get(1);
        }

        public int HashCode()
        {
            return Convert.ToInt32(Math.Pow(fontName.GetHashCode(), Math.Round(fontSize * 10)));
        }

        public bool Equals(Object obj)
        {
            if (obj is TextStyle ts)
            {
                return fontName.Equals(ts.fontName) && Math.Abs(fontSize - ts.fontSize) < 0.05;
            }
            return false;
        }
    }
}
