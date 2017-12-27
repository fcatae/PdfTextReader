using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader
{
    class TableCell : IBlock
    {
        public string Text { get; set; }
        public float X { get; set; }
        public float B { get; set; }
        public float H { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public int Op { get; set; }
        public string GetText() => "<<TABELA>>";
        public float GetX() => X;
        public float GetH() => H;
        public float GetWidth() => Width;
        public float GetHeight() => Height;
    }
}
