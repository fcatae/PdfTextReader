using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace PdfTextReader.Structure
{
    class ProcessStructure2
    {
        public List<TextStructure> ProcessParagraph(List<TextLine> lineSet)
        {
            throw new InvalidOperationException();
        }

        public List<TextLine> ProcessLine(BlockSet items)
        {
            throw new InvalidOperationException();
        }

        #region Codigo Temporario
        public List<TextLine> ProcessLine(List<Lucas_Testes.Helpers.MainItem> items, List<BlockSet> _list = null)
        {
            List<TextLine> lines = new List<TextLine>();
            List<Lucas_Testes.Helpers.MainItem> line = new List<Lucas_Testes.Helpers.MainItem>();


            List<Lucas_Testes.Helpers.MainItem> ItemsInBlockSets;


            foreach (var b in _list)
            {
                ItemsInBlockSets = new List<Lucas_Testes.Helpers.MainItem>();
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

                        lines.Add(LineTreatment(line, b));
                        line = new List<Lucas_Testes.Helpers.MainItem>
                        {
                            item
                        };
                    }
                }
                if (line.Count > 0)
                    lines.Add(LineTreatment(line, b));
            }

            lines = ProcessBreakLineDistance(lines);
            return lines;
        }

        TextLine LineTreatment(List<Lucas_Testes.Helpers.MainItem> finalBlocks, BlockSet b)
        {
            TextLine line = new TextLine();

            var item = finalBlocks[0] as Lucas_Testes.Helpers.TextItem;
            if (item != null)
            {
                line.FontName = item.fontName;
                line.FontSize = Decimal.Round(Convert.ToDecimal(item.fontSize), 2);
                line.Text = GetText(finalBlocks);
                line.MarginLeft = GetMargins(finalBlocks, b)[0];
                line.MarginRight = GetMargins(finalBlocks, b)[1];
            }
            return line;
        }

        List<TextLine> ProcessBreakLineDistance(List<TextLine> lines)
        {
            for (int i = 0; i < lines.Count; i++)
            {
               // ???? Onde pego a porra da posição do item para medir a distancia?
            }
            return lines;
        }

        List<float> GetGraphicPositions(List<Lucas_Testes.Helpers.MainItem> items)
        {
            float left = Single.MaxValue;
            float right = 0;
            float top = 0;
            float bottom = Single.MaxValue;
            foreach (Lucas_Testes.Helpers.MainItem item in items)
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
            return new List<float>() { left, bottom, right - left, top - bottom };
        }

        List<decimal> GetMargins(List<Lucas_Testes.Helpers.MainItem> finalBlocks, BlockSet b)
        {
            var lineX0 = GetGraphicPositions(finalBlocks)[0];
            var lineX1 = GetGraphicPositions(finalBlocks)[0] + GetGraphicPositions(finalBlocks)[2];

            //Pegando dados do bloco atual
            var blockX0 = b.GetX();
            var blockX1 = b.GetWidth() + b.GetX();

            //tratando as margens da linha
            var marginLeft = lineX0 - blockX0;
            var marginRight = blockX1 - lineX1;

            return new List<decimal>() { Decimal.Round(Convert.ToDecimal(marginLeft), 2), Decimal.Round(Convert.ToDecimal(marginRight), 2) };
        }

        private static string GetText(List<Lucas_Testes.Helpers.MainItem> items)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in items)
            {
                var i = item as Lucas_Testes.Helpers.TextItem;
                if (item != null)
                {
                    sb.Append(i.text);
                }
            }
            return sb.ToString();
        }

        public static bool IsInsideBlock(Lucas_Testes.Helpers.MainItem p, BlockSet b)
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

        static bool AreOnSameLine(Lucas_Testes.Helpers.MainItem i1, Lucas_Testes.Helpers.MainItem i2)
        {
            return Math.Abs(i1.GetLL().GetY() - i2.GetLL().GetY()) <= Lucas_Testes.Helpers.MainItem.itemPositionTolerance;
        }


        #endregion

    }
}
