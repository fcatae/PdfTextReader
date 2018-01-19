using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Base
{
    class MarkLine : IBlock
    {
        public const int ORANGE = 10210;
        public const int YELLOW = 10220;
        public const int PURPLE = 10101;

        public float X { get; set; }
        public float B { get; set; }
        public float H { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public string GetText() => throw new NotImplementedException();
        public float GetX() => X;
        public float GetH() => H;
        public float GetWidth() => Width;
        public float GetHeight() => Height;
        public float GetWordSpacing() => throw new NotImplementedException();
        public float LineWidth { get; set; }
        public int Color { get; set; }
    }
}
