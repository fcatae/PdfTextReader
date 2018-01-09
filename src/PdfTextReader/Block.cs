using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader
{
    public interface IBlock
    {
        string GetText();
        float GetX();
        float GetH();
        float GetWidth();
        float GetHeight();
        float GetWordSpacing();
    }

    public class Block : IBlock
    {
        public string Text { get; set; }
        public float X { get; set; }
        public float B { get; set; }
        public float H { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float Lower { get; set; }
        public string FontName { get; set; }
        public string FontFullName { get; set; }
        public string FontStyle { get; set; }
        public float FontSize { get; set; }
        public float WordSpacing { get; set; }
        public int Intensity { get; private set; }

        public string GetText() => Text;
        public float GetX() => X;
        public float GetH() => H;
        public float GetWidth() => Width;
        public float GetHeight() => Height;
        public float GetWordSpacing() => WordSpacing;

        public void SetHighlight(int intensity)
        {
            Intensity = intensity;
        }

        public static bool HasOverlap(IBlock a, IBlock b)
        {
            float a_x1 = a.GetX();
            float a_x2 = a.GetX() + a.GetWidth();
            float a_y1 = a.GetH();
            float a_y2 = a.GetH() + a.GetHeight();

            float b_x1 = b.GetX();
            float b_x2 = b.GetX() + b.GetWidth();
            float b_y1 = b.GetH();
            float b_y2 = b.GetH() + b.GetHeight();

            return (HasOverlap(a_x1, a_x2, b_x1, b_x2) && HasOverlap(a_y1, a_y2, b_y1, b_y2));
        }

        static bool HasOverlap(float a1, float a2, float b1, float b2)
        {
            if ((b1 > b2) || (a1 > a2))
                throw new InvalidOperationException();

            bool separated = ((a1 < b1) && (a2 < b1)) ||
                             ((a1 > b2) && (a2 > b2));
            return !separated;
        }

        public static bool HasOverlapY(IBlock block, float b1, float b2)
        {
            if (b1 > b2)
                throw new InvalidOperationException();

            float a1 = block.GetH();
            float a2 = block.GetH() + block.GetHeight();

            bool separated = ((a1 < b1) && (a2 < b1)) ||
                             ((a1 > b2) && (a2 > b2));
            return !separated;
        }
    }

}
