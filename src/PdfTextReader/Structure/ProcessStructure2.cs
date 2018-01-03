using System;
using System.Collections.Generic;
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
        public static List<TextLine> ProcessLine(List<Lucas_Testes.Helpers.MainItem> items, List<BlockSet> _list = null)
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

            return lines;
        }

        static TextLine LineTreatment(List<Lucas_Testes.Helpers.MainItem> finalBlocks, BlockSet b)
        {
            TextLine line = new TextLine()
            {
                
            };
            return line;
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
