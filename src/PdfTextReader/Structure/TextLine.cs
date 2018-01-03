using PdfTextReader.Interfaces;
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
        public float MarginRight { get; set; }
        public float MarginLeft { get; set; }
        public decimal Distace2NextLine { get; set; }


        public TextLine()
        {

        }
    }
}
