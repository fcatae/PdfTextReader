using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Base
{
    class TextLine : IBlock
    {
        public string FontName { get; set; }
        public float FontSize { get; set; }
        public string Text { get; set; }
        public float MarginRight { get; set; }
        public float MarginLeft { get; set; }
        public float? AfterSpace { get; set; }
        public float? BeforeSpace { get; set; }
        public string FontStyle { get; set; }
        public bool HasBackColor { get; set; }

        public IBlock Block { get; set; }

        public TextLine()
        {
        }

        public string GetText() => Block.GetText();

        public float GetX() => Block.GetX();
        public float GetH() => Block.GetH();

        public float GetWidth() => Block.GetWidth();

        public float GetHeight() => Block.GetHeight();

        public float GetWordSpacing() => Block.GetWordSpacing();   }    
}
