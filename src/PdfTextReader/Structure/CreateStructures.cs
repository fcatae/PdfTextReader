using System;
using System.Collections.Generic;
using System.Text;
using PdfTextReader.PDFCore;
using System.Linq;

namespace PdfTextReader.Structure
{
    class CreateStructures : IConvertBlock
    {
        public TextSet ConvertBlock(BlockPage page)
        {
            var textSet = new TextSet();

            foreach(var bset in page.AllBlocks)
            {
                var lines = ProcessLine((BlockSet<IBlock>)bset);

                textSet.Append(lines);
            }

            return textSet;
        }

        List<TextLine> ProcessLine(BlockSet<IBlock> bset)
        {
            var items = bset;

            float minx = bset.GetX();
            float maxx = bset.GetX() + bset.GetWidth();
            float last_y = float.NaN;
            TextLine last_tl = null;

            var lines = new List<TextLine>();

            foreach (var it in items)
            {
                var bl = (PDFCore.BlockLine2)it;

                var tl = new TextLine
                {
                    FontName = bl.FontName,
                    FontSize = (decimal)bl.FontSize,
                    FontStyle = bl.FontStyle,
                    Text = bl.Text,
                    MarginLeft = Decimal.Round(Convert.ToDecimal(bl.GetX() - minx), 2),
                    MarginRight = Decimal.Round(Convert.ToDecimal(maxx - (bl.GetX() + bl.GetWidth())), 2),
                    Breakline = 0
                };

                lines.Add(tl);

                if (last_tl != null)
                {
                    if (float.IsNaN(last_y))
                        throw new InvalidOperationException();

                    float a = bl.GetHeight();
                    float b = bl.FontSize;
                    float diff = last_y - bl.GetH();
                    last_tl.Breakline = (decimal)(last_y - bl.GetH() - bl.FontSize);
                }

                last_tl = tl;
                last_y = bl.GetH();
            }

            return lines.ToList();
        }
    }
}
