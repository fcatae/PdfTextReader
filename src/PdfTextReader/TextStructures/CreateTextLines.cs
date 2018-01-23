using System;
using System.Collections.Generic;
using System.Text;
using PdfTextReader.PDFCore;
using System.Linq;
using PdfTextReader.Base;

namespace PdfTextReader.TextStructures
{
    class CreateTextLines : IConvertBlock
    {
        public IEnumerable<TextLine> ProcessPage(int pageNumber, BlockPage page)
        {
            int blockId = 0;

            foreach (var bset in page.AllBlocks)
            {                
                var blockArea = bset as IBlockSet<IBlock>;
                var pageInfo = new TextPageInfo()
                {
                    PageNumber = pageNumber,
                    BlockId = blockId
                };

                if (bset is MarkLine)
                    continue;

                if (bset is ImageBlock || bset is TableSet)
                    continue;

                // TODO: fix this bug
                if (blockArea.First() is TableCell)
                {
                    PdfReaderException.Throw("blockArea.First() is TableCell");
                    continue;
                }                    

                var lines = ProcessLine(blockArea, pageInfo);

                foreach(var l in lines)
                {
                    yield return l;
                }

                blockId++;
            }
        }

        List<TextLine> ProcessLine(IBlockSet<IBlock> bset, TextPageInfo pageInfo)
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
                    FontSize = bl.FontSize,
                    FontStyle = bl.FontStyle,
                    Text = bl.Text,
                    MarginLeft = bl.GetX() - minx,
                    MarginRight = maxx - (bl.GetX() + bl.GetWidth()),
                    BeforeSpace = (last_tl != null) ? (float?)(last_y - bl.GetH() - bl.FontSize) : null,
                    AfterSpace = null,
                    Block = bl,
                    HasBackColor = bl.HasBackColor,
                    PageInfo = pageInfo
                };

                tl.CenteredAt = 0.5f * (tl.MarginLeft - tl.MarginRight);

                lines.Add(tl);

                if (last_tl != null)
                {
                    if (float.IsNaN(last_y))
                        PdfReaderException.AlwaysThrow("float.IsNaN(last_y)");

                    float a = bl.GetHeight();
                    float b = bl.FontSize;
                    float diff = last_y - bl.GetH();
                    last_tl.AfterSpace = (last_y - bl.GetH() - bl.FontSize);
                }

                last_tl = tl;
                last_y = bl.GetH();
            }

            return lines.ToList();
        }
    }
}
