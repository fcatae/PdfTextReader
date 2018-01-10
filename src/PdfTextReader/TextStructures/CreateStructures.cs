using System;
using System.Collections.Generic;
using System.Text;
using PdfTextReader.PDFCore;
using System.Linq;
using PdfTextReader.Base;

namespace PdfTextReader.TextStructures
{
    class CreateStructures : IConvertBlock
    {
        //public TextSet ConvertBlock(BlockPage page)
        //{
        //    var textSet = new TextSet();

        //    foreach(var bset in page.AllBlocks)
        //    {
        //        if (bset is ImageBlock || bset is TableSet)
        //            continue;

        //        var lines = ProcessLine((IBlockSet<IBlock>)bset);

        //        textSet.Append(lines);
        //    }

        //    return textSet;
        //}

        public IEnumerable<TextLine> ProcessPage(BlockPage page)
        {
            return ProcessLine(page.AllBlocks);
        }

        List<TextLine> ProcessLine(IBlockSet<IBlock> bset)
        {
            var items = bset;

            float minx = bset.GetX();
            float maxx = bset.GetX() + bset.GetWidth();
            float last_y = float.NaN;
            TextLine last_tl = null;

            var lines = new List<TextLine>();

            foreach (var it in items)
            {
                var bl = (BlockLine)it;

                var tl = new TextLine
                {
                    FontName = bl.FontName,
                    FontSize = (decimal)bl.FontSize,
                    FontStyle = bl.FontStyle,
                    Text = bl.Text,
                    MarginLeft = Decimal.Round(Convert.ToDecimal(bl.GetX() - minx), 2),
                    MarginRight = Decimal.Round(Convert.ToDecimal(maxx - (bl.GetX() + bl.GetWidth())), 2),
                    VSpacing = (last_tl != null) ? (decimal?)(last_y - bl.GetH() - bl.FontSize) : null,
                    Breakline = null,
                    Block = bl
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
