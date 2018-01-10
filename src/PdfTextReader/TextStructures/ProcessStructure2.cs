using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace PdfTextReader.TextStructures
{
    class ProcessStructure2
    {
        decimal Tolerance = 3;
        public IEnumerable<TextStructure> ProcessParagraph(IEnumerable<TextLine> lineSet)
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
                    ExecutionStats.TextInfo infos = GetTextInfos(lines);
                    TextStructure structure = new TextStructure()
                    {
                        FontName = infos.FontName,
                        FontSize = infos.FontSize,
                        FontStyle = infos.FontStyle,
                        Text = GetText(lines),
                        TextAlignment = GetParagraphTextAlignment(lines),
                        Lines = lines,
                        MarginLeft = lines[0].MarginLeft,
                        MarginRight = lines[0].MarginRight
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
                ExecutionStats.TextInfo infos = GetTextInfos(lines);
                TextStructure structure = new TextStructure()
                {
                    FontName = infos.FontName,
                    FontSize = infos.FontSize,
                    FontStyle = infos.FontStyle,
                    Text = GetText(lines),
                    TextAlignment = GetParagraphTextAlignment(lines),
                    Lines = lines,
                    MarginLeft = lines[0].MarginLeft,
                    MarginRight = lines[0].MarginRight
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
                return TextAlignment.UNKNOWN;
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
        public static List<ExecutionStats.TextInfo> GetAllTextInfos(List<TextLine> lines)
        {
            List<ExecutionStats.TextInfo> Styles = new List<ExecutionStats.TextInfo>();

            foreach (TextLine line in lines)
            {
                var result = Styles.Where(i => i.FontName == line.FontName && i.FontSize == Decimal.Round(Convert.ToDecimal(line.FontSize), 2)).FirstOrDefault();
                if (result == null)
                {
                    Styles.Add(new ExecutionStats.TextInfo(line));
                }
            }
            return Styles;
        }

        public static ExecutionStats.TextInfo GetTextInfos(List<TextLine> lines)
        {
            var linesGrouped = lines.GroupBy(line => new { line.FontName, line.FontStyle, line.FontSize });
            var result = linesGrouped.OrderByDescending(group => group.Count()).ToList().FirstOrDefault().Key;
            return new ExecutionStats.TextInfo(result.FontName, result.FontStyle, result.FontSize);
        }
        
    }
}
