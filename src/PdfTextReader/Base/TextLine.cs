using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Base
{
    public class TextLine : IBlock
    {
        public string FontName { get; set; }
        public decimal FontSize { get; set; }
        public string Text { get; set; }
        public decimal MarginRight { get; set; }
        public decimal MarginLeft { get; set; }
        public decimal? Breakline { get; set; }
        public decimal? VSpacing { get; set; }
        public string FontStyle { get; set; }

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
