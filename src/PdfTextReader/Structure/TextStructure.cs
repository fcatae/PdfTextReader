using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Structure
{
    public class TextStructure
    {
        public string FontName { get; set; }
        public decimal FontSize { get; set; }
        public string Text { get; set; }
        public TextAlignment TextAlignment { get; set; }

        //For ContentType
        public List<TextLine> Lines { get; set; }

        public int CountLines() => Lines.Count;
    }
}
