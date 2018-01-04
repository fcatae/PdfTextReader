using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace PdfTextReader.Structure
{
    class ProcessStructure2
    {
        decimal Tolerance = 3;
        public List<TextStructure> ProcessParagraph(IEnumerable<TextLine> lineSet)
        {
            List<TextStructure> structures = new List<TextStructure>();
            List<TextLine> lines = new List<TextLine>();


            foreach (TextLine line in lineSet)
            {
                //If structure is empty, add the first line
                if (lines.Count == 0)
                {
                    lines.Add(line);
                    continue;
                }
                //If this line it is in the same structure, add it. Otherwise, it is other structure
                if (AreInSameStructure(lines[lines.Count - 1], line))
                {
                    lines.Add(line);
                }
                else
                {
                    Stats.TextInfo infos = GetTextInfos(lines);
                    TextStructure structure = new TextStructure()
                    {
                        FontName = infos.FontName,
                        FontSize = infos.FontSize,
                        Text = GetText(lines),
                        TextAlignment = GetParagraphTextAlignment(lines),
                        Lines = lines
                    };
                    structures.Add(structure);

                    lines = new List<TextLine>()
                    {
                        line
                    };
                }
            }
            if (lines.Count > 0)
            {
                Stats.TextInfo infos = GetTextInfos(lines);
                TextStructure structure = new TextStructure()
                {
                    FontName = infos.FontName,
                    FontSize = infos.FontSize,
                    Text = GetText(lines),
                    TextAlignment = GetParagraphTextAlignment(lines),
                    Lines = lines
                };
                structures.Add(structure);
            }
            return structures;
        }

        private TextAlignment GetParagraphTextAlignment(List<TextLine> lines)
        {

            List<TextAlignment> alignments = new List<TextAlignment>();
            foreach (var item in lines)
            {
                alignments.Add(GetLineTextAlignment(item));
            }

            var listGrouped = alignments.GroupBy(a => a);
            var result = listGrouped.OrderByDescending(group => group.Count()).ToList().FirstOrDefault().Key;
            return result;
        }

        TextAlignment GetLineTextAlignment(TextLine line)
        {
            if (line.MarginLeft - line.MarginRight <= Tolerance && line.MarginRight < Tolerance)
            {
                return TextAlignment.JUSTIFY;
            }
            else if (line.MarginLeft - line.MarginRight <= Tolerance && line.MarginLeft - line.MarginRight > -Tolerance && line.MarginRight > Tolerance)
            {
                return TextAlignment.CENTER;
            }
            else if (line.MarginLeft - line.MarginRight > 0)
            {
                return TextAlignment.RIGHT;
            }
            else if (line.MarginLeft - line.MarginRight < 0)
            {
                return TextAlignment.LEFT;
            }
            else
            {
                return TextAlignment.UNKNOW;
            }
        }


        //Verify if two lines are in the same structure
        bool AreInSameStructure(TextLine l1, TextLine l2)
        {
            //Have the same Font Name and Font Size? It is the same line
            if (l1.FontName == l2.FontName && l1.FontSize == l2.FontSize)
            {
                //The breakline for the first line is minor than the font size of second line? It is the same line
                if (l1.Breakline < l2.FontSize / Tolerance)
                {
                    //The second line has a paragraph margin and the first line is not centered or right alignment? It is NOT the same line
                    if ((GetLineTextAlignment(l1) == TextAlignment.JUSTIFY || GetLineTextAlignment(l1) == TextAlignment.LEFT) && GetLineTextAlignment(l2) == TextAlignment.RIGHT)
                    {
                        return false;
                    }
                    else // Otherwise, it is the same line
                    {
                        //All signatures are not centered. They have right alignment with a huge right margin
                        //If the second line has right alignment and the right margin is bigger than Tolerance (3) * 2? It is NOT the same line
                        if ((GetLineTextAlignment(l2) == TextAlignment.RIGHT) && l2.MarginRight > Tolerance * 2)
                        {
                            return false;
                        }
                        return true; // Otherwise it is the same line
                    }
                }
            }
            return false;
        }


        //Get all texts inside the structure and put /n and /t
        string GetText(List<TextLine> lines)
        {
            StringBuilder sb = new StringBuilder();

            foreach (TextLine line in lines)
            {
                if (lines.Count > 1)
                {
                    if (GetLineTextAlignment(lines[0]) == TextAlignment.RIGHT && GetLineTextAlignment(lines[1]) == TextAlignment.JUSTIFY)
                    {
                        sb.Append("\t");
                    }
                }
                sb.Append($"{line.Text}\n");
            }

            return sb.ToString();
        }


        //Analyze Font
        public static List<Stats.TextInfo> GetAllTextInfos(List<TextLine> lines)
        {
            List<Stats.TextInfo> Styles = new List<Stats.TextInfo>();

            foreach (TextLine line in lines)
            {
                var result = Styles.Where(i => i.FontName == line.FontName && i.FontSize == Decimal.Round(Convert.ToDecimal(line.FontSize), 2)).FirstOrDefault();
                if (result == null)
                {
                    Styles.Add(new Stats.TextInfo(line));
                }
            }
            return Styles;
        }

        public static Stats.TextInfo GetTextInfos(List<TextLine> lines)
        {
            var linesGrouped = lines.GroupBy(line => new { line.FontName, line.FontSize });
            var result = linesGrouped.OrderByDescending(group => group.Count()).ToList().FirstOrDefault().Key;
            return new Stats.TextInfo(result.FontName, result.FontSize);
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
