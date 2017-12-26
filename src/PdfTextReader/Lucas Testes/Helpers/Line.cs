using iText.Kernel.Geom;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Lucas_Testes.Helpers
{
    public class Line : MainItem
    {
        public Line(List<MainItem> items) : base()
        {
            rectangle = GetItemsRect(items);
            color = items[0].GetColor();
        }

        private static Rectangle GetItemsRect(List<MainItem> items)
        {
            float left = Single.MaxValue;
            float right = 0;
            float top = 0;
            float bottom = Single.MaxValue;
            foreach (MainItem item in items)
            {
                //item rect
                var iLeft = item.GetRectangle().GetLeft();
                var iRigth = item.GetRectangle().GetRight();
                var iTop = item.GetRectangle().GetTop();
                var iBottom = item.GetRectangle().GetBottom();
                var iWidth = iRigth - iLeft;
                var iHeight = iTop - iBottom;


                if (item.GetRectangle().GetLeft() < left)
                    left = item.GetRectangle().GetLeft();
                if (item.GetRectangle().GetRight() > right)
                    right = item.GetRectangle().GetRight();
                if (item.GetRectangle().GetTop() > top)
                    top = item.GetRectangle().GetTop();
                if (item.GetRectangle().GetBottom() < bottom)
                    bottom = item.GetRectangle().GetBottom();
            }
            return new Rectangle(left, bottom, right - left, top - bottom);
        }

        public static List<Line> GetLines(List<MainItem> items)
        {
            List<Line> lines = new List<Line>();
            List<MainItem> line = new List<MainItem>();
            foreach (var item in items)
            {
                if (line.Count == 0)
                {
                    line.Add(item);
                    continue;
                }
                if (AreOnSameLine(line[line.Count - 1], item))
                {
                    line.Add(item);
                }
                else
                {
                    lines.Add(new Line(line));
                    line = new List<MainItem>
                    {
                        item
                    };
                }
            }
            if (line.Count > 0)
                lines.Add(new Line(line));
            return lines;
        }

        static bool AreOnSameLine(MainItem i1, MainItem i2)
        {
            return Math.Abs(i1.GetLL().GetY() - i2.GetLL().GetY()) <= MainItem.itemPositionTolerance;
        }
    }
}
