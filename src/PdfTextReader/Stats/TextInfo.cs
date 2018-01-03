using System;
using System.Collections.Generic;
using System.Text;

//Temp
using PdfTextReader.Lucas_Testes.Helpers;

namespace PdfTextReader.Stats
{
    public class TextInfo
    {
        public string FontName;
        public decimal FontSize;
        public string Text;

        public TextInfo(TextItem item)
        {
            this.FontName = item.fontName;
            this.FontSize = Decimal.Round(Convert.ToDecimal(item.fontSize),2);
            this.Text = item.text;
        }

        public TextInfo(LineItem item)
        {
            this.FontName = item.fontName;
            this.FontSize = Decimal.Round(Convert.ToDecimal(item.fontSize), 2);
            this.Text = item.Text;
        }
    }
}
