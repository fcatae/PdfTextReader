using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Structure
{
    public class TextLine
    {
        public string FontName { get; set; }
        public decimal FontSize { get; set; }
        public string Text { get; set; }
        public decimal MarginRight { get; set; }
        public decimal MarginLeft { get; set; }
        public decimal Breakline { get; set; }


        public TextLine()
        {

        }
    }
}
