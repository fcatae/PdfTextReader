using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class BlockLine : BlockSet
    {
        public string FontName { get; set; }
        public float FontSize { get; set; }
        public string Text => GetText();

    }
}
