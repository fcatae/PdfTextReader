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
            string font = String.Empty;
            try
            {
                font = textRenderInfo.GetFont().GetFontProgram().GetFontNames().GetFullName()[0][3];
            }
            catch (Exception ex) { }
            if (String.IsNullOrWhiteSpace(font))
            {
                try
                {
                    font = textRenderInfo.GetFont().GetFontProgram().GetFontNames().GetFamilyName()[0][3];
                }
                catch (Exception ex) { }
            }
            if (String.IsNullOrWhiteSpace(font))
            {
                try
                {
                    font = textRenderInfo.GetFont().GetFontProgram().GetFontNames().GetFontName();
                }
                catch (Exception ex) { }
            }
            if (font.Contains("+"))
                font = font.Substring(font.IndexOf("+") + 1, font.Length - font.IndexOf("+") - 1);
            if (font.Contains("-"))
            {
                if (font.IndexOf("-") == 6)
                {
                    font = font.Substring(0, font.IndexOf("-"));
                }
            }
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
