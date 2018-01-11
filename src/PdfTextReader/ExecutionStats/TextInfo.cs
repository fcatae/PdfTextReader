using System;
using System.Collections.Generic;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.ExecutionStats
{
    class TextInfo
    {
        public string FontName;
        public float FontSize;
        public string FontStyle;
        public string Text;

        public TextInfo(TextLine line)
        {
            this.FontName = line.FontName;
            this.FontSize = line.FontSize;
            this.FontStyle = line.FontStyle;
            this.Text = line.Text;
        }

        public TextInfo(string fontName, string fontStyle, float fontSize)
        {
            this.FontName = fontName;
            this.FontStyle = fontStyle;
            this.FontSize = fontSize;
            this.Text = "";
        }
    }
}
