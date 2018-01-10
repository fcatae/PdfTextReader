using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class ImageBlock : IBlock
    {
        public float X;
        public float H;
        public float Width;
        public float Height;
        public string ResourceName;

        public string GetText() => throw new InvalidOperationException();
        public float GetX() => X;
        public float GetH() => H;
        public float GetWidth() => Width;
        public float GetHeight() => Height;
        public float GetWordSpacing() => throw new InvalidOperationException();
    }
}
