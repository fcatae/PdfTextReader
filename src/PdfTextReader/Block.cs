using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader
{
    interface IBlock
    {
        string GetText();
        float GetX();
        float GetH();
        float GetWidth();
        float GetHeight();
    }

    class Block : IBlock
    {
        public string Text { get; set; }
        public float X { get; set; }
        public float B { get; set; }
        public float H { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float Lower { get; set; }
        public string FontName { get; set; }
        public float FontSize { get; set; }

        public string GetText() => Text;
        public float GetX() => X;
        public float GetH() => H;
        public float GetWidth() => Width;
        public float GetHeight() => Height;
    }
}
