using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Base
{
    class TableCell : IBlock
    {
        const float DARKCOLOR_THRESHOLD = 0.5f;
        public static bool HasDarkColor(TableCell t) => (t.BgColor < DARKCOLOR_THRESHOLD);
        public static bool HasWhiteColor(TableCell t) => (t.BgColor == 1);
        
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
        public float GetWordSpacing() => X;
        public float LineWidth { get; set; }
        public float BgColor { get; set; }

    }
}
