using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Stats
{
    public class TextInfo
    {
        public string FontName;
        public decimal FontSize;
        public string Text;

        public TextInfo(Structure.TextLine line)
        {
            this.FontName = line.FontName;
            this.FontSize = Decimal.Round(Convert.ToDecimal(line.FontSize),2);
            this.Text = line.Text;
        }

        public TextInfo(string fontName, decimal fontSize)
        {
            this.FontName = fontName;
            this.FontSize = fontSize;
            this.Text = "";
        }
    }
}
