using iText.Kernel.Geom;
using iText.Kernel.Colors;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Lucas_Testes.Helpers
{
    public abstract class MainItem : IComparable<MainItem>
    {
        public static float itemPositionTolerance = 3f;
        public static float ColumnPositionTolerance = 10f;

        protected Rectangle rectangle;

        protected Color color;

        protected MainItem()
        {
            this.rectangle = rectangle;
            this.color = color;
        }

        public Rectangle GetRectangle()
        {
            return rectangle;
        }

        public Color GetColor()
        {
            return color;
        }

        public Point GetLL()
        {
            return new Point(GetRectangle().GetLeft(), GetRectangle().GetBottom());
        }

        public int CompareTo(MainItem o)
        {
            double left = GetLL().GetX();
            double bottom = GetLL().GetY();
            double oLeft = o.GetLL().GetX();
            double oBottom = o.GetLL().GetY();
            if (bottom - oBottom > itemPositionTolerance)
                return -1;
            else if (oBottom - bottom > itemPositionTolerance)
                return 1;
            else
                return 0;     //Double.Equals(left, oLeft);
        }
    }
}
