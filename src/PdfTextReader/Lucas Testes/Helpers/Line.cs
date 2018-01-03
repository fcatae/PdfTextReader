using iText.Kernel.Geom;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Lucas_Testes.Helpers
{
    public class LineItem : MainItem
    {
        public enum Margin { RIGTH, LEFT, CENTER }

        public Margin margin;
        public bool isParagraph;
        public bool isUpperLetter;
        public string Text;
        public bool isSignture;
        public string fontName;
        public float fontSize;

        public LineItem(List<MainItem> items) : base()
        {
            rectangle = GetItemsRect(items);
            color = items[0].GetColor();
            Text = GetText(items);
            fontSize = GetFontSize(items);
            fontName = GetFontName(items);
        }

        private static string GetFontName(List<MainItem> items)
        {
            if (items[0].GetType() == typeof(TextItem))
            {
                var i = items[0] as TextItem;
                return i.fontName;
            }
            return String.Empty;
        }

        private static float GetFontSize(List<MainItem> items)
        {
            if (items[0].GetType() == typeof(TextItem))
            {
                var i = items[0] as TextItem;
                return i.fontSize;
            }
            return 0;
        }

        private static string GetText(List<MainItem> items)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in items)
            {
                if (item.GetType() == typeof(TextItem))
                {
                    var i = item as TextItem;
                    sb.Append(i.text);
                }
            }
            return sb.ToString();
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
                    }
                    else
                    {

                        lines.Add(LineTreatment(new LineItem(line), b));
                        line = new List<MainItem>
                        {
                            item
                        };
                    }
                }
                if (line.Count > 0)
                    lines.Add(LineTreatment(new LineItem(line), b));
            }

            return lines;
        }

        static LineItem LineTreatment(LineItem finalLine, BlockSet b)
        {
            //Pegando dados da linha atual
            var lineX0 = finalLine.GetRectangle().GetLeft();
            var lineX1 = finalLine.GetRectangle().GetWidth() + finalLine.GetRectangle().GetLeft();

            //Pegando dados do bloco atual
            var blockX0 = b.GetX();
            var blockX1 = b.GetWidth() + b.GetX();

            //tratando as margens da linha
            var marginLeft = lineX0 - blockX0;
            var marginRight = blockX1 - lineX1;

            //Vendo o alinhamento
            if (marginLeft - marginRight <= MainItem.itemPositionTolerance && marginLeft - marginRight >= -MainItem.itemPositionTolerance && marginLeft > 15)
            {
                finalLine.margin = Margin.CENTER;
            }
            else if (marginRight > 15)
            {
                finalLine.margin = Margin.LEFT;
            }
            else
            {
                finalLine.margin = Margin.RIGTH;
            }
            return ParagraphTreatment(finalLine, b);
        }

        static LineItem ParagraphTreatment (LineItem finalLine, BlockSet b)
        {
            //Pegando dados da linha atual
            var lineX0 = finalLine.GetRectangle().GetLeft();
            var lineX1 = finalLine.GetRectangle().GetWidth() + finalLine.GetRectangle().GetLeft();

            //Pegando dados do bloco atual
            var blockX0 = b.GetX();
            var blockX1 = b.GetWidth() + b.GetX();

            //tratando as margens da linha
            var marginLeft = lineX0 - blockX0;
            var marginRight = blockX1 - lineX1;


            if (marginLeft > 18 && marginRight < MainItem.itemPositionTolerance)
            {
                finalLine.isParagraph = true;
            }


            return IsSignture(finalLine, b);
        }

        static LineItem IsSignture(LineItem finalLine, BlockSet b)
        {
            //Pegando dados da linha atual
            var lineX0 = finalLine.GetRectangle().GetLeft();
            var lineX1 = finalLine.GetRectangle().GetWidth() + finalLine.GetRectangle().GetLeft();

            //Pegando dados do bloco atual
            var blockX0 = b.GetX();
            var blockX1 = b.GetWidth() + b.GetX();

            //tratando as margens da linha
            var marginLeft = lineX0 - blockX0;
            var marginRight = blockX1 - lineX1;

            if (marginLeft > 60 && marginLeft > 10)
            {
                finalLine.isSignture = true;
            }

            return IsUpper(finalLine);
        }

        static LineItem IsUpper(LineItem finalLine)
        {
            if (finalLine.Text != null)
            {
                if (finalLine.Text == finalLine.Text.ToUpper())
                {
                    finalLine.isUpperLetter = true;
                }
            }
            return finalLine;
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
