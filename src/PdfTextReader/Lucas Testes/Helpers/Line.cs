using iText.Kernel.Geom;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Lucas_Testes.Helpers
{
    public class LineItem : MainItem
    {
        public LineItem(List<MainItem> items) : base()
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

        public static bool IsInsideBlock(MainItem p, BlockSet b)
        {
            if (p.GetRectangle().GetX() >= b.GetX())
            {
                if (p.GetRectangle().GetY() >= b.GetH())
                {
                    if (p.GetRectangle().GetWidth() < b.GetWidth())
                    {
                        if (p.GetRectangle().GetHeight() < b.GetHeight())
                        {
                            if (p.GetRectangle().GetX() <= b.GetWidth() + b.GetX())
                            {
                                if (p.GetRectangle().GetY() <= b.GetHeight() + b.GetH())
                                {
                                    return true;
                                }
                                return false;
                            }
                            return false;
                        }
                        return false;
                    }
                    return false;
                }
                return false;
            }
            return false;
        }

        public static List<LineItem> GetLines(List<MainItem> items, List<BlockSet> _list = null)
        {
            List<LineItem> lines = new List<LineItem>();
            List<MainItem> line = new List<MainItem>();


            List<MainItem> ItemsInBlockSets;


            foreach (var b in _list)
            {
                ItemsInBlockSets = new List<MainItem>();
                foreach (var i in items)
                {
                    if (IsInsideBlock(i, b))
                    {
                        ItemsInBlockSets.Add(i);
                    }
                }

                foreach (var item in ItemsInBlockSets)
                {
                    if (line.Count == 0)
                    {
                        line.Add(item);
                        continue;
                    }
                    if (AreOnSameLine(line[line.Count - 1], item))
                    {
                        line.Add(item);
                        //if (AreOnSameColumn(line[line.Count - 1], item))
                        //{
                        //    line.Add(item);
                        //}
                        //else
                        //{
                        //    continue;
                        //}
                    }
                    else
                    {
                        lines.Add(new LineItem(line));
                        line = new List<MainItem>
                    {
                        item
                    };
                    }
                }
                if (line.Count > 0)
                    lines.Add(new LineItem(line));
            }

            return lines;
        }

        static bool AreOnSameLine(MainItem i1, MainItem i2)
        {
            return Math.Abs(i1.GetLL().GetY() - i2.GetLL().GetY()) <= MainItem.itemPositionTolerance;
        }

        static bool AreOnSameColumn(MainItem i1, MainItem i2)
        {
            return Math.Abs(i1.GetLL().GetX() - i2.GetLL().GetX()) <= MainItem.ColumnPositionTolerance;
        }
    }
}
