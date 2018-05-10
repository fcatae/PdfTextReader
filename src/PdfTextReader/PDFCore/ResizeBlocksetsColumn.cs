using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.PDFCore
{
    class ResizeBlocksetsColumn : IProcessBlock
    {
        const float COLUMN_DISTANCE = 10f;

        public BlockPage Process(BlockPage page)
        {
            var blocksets = page.AllBlocks.ToList();

            if (blocksets.Count == 0)
                return page;

            // implemented ONLY for 3 columns
            if (blocksets.Count != 3)
                return page;

            var columns = page.AllBlocks.OrderBy(b => b.GetX()).ToArray();
            float maxColumn = page.AllBlocks.Max(b => b.GetWidth());

            float x1 = page.AllBlocks.GetX();
            float x2 = page.AllBlocks.GetX() + page.AllBlocks.GetWidth();
            float dx = page.AllBlocks.GetWidth() + 2;

            int id = 0;

            var resizedColumns = columns.Select(b => new
            {
                ID = id++,
                X = (int)(6.0 * ((b.GetX() - x1) / dx) + 0.5),
                W = (int)(6.0 * (b.GetWidth() / dx) + 0.5),
                B = b
            })
                .Select(d =>
                {
                    // may receive multiples - confusing...
                    var original = (IEnumerable<IBlock>)d.B;

                    if ((original is TableSet) || (original is ImageBlock))
                        return d.B;

                    int nextId = d.ID + 1;

                    if (d.ID >= 3)   // only first and second
                        return d.B;

                    if (d.W == 1)           // small column
                    {
                        var block = d.B;
                        float new_x2 = columns[nextId].GetX() - COLUMN_DISTANCE;
                        float old_x2 = block.GetX() + block.GetWidth();
                        float diff = new_x2 - old_x2;

                        if (diff < 0)
                            PdfReaderException.Warning("decreasing the column size");

                        var replace = new BlockSet2<IBlock>(original, block.GetX(), block.GetH(), new_x2, block.GetH() + block.GetHeight());
                        return replace;
                    }

                    return d.B;
                }).ToArray();

            var newpage = new BlockPage();
            newpage.AddRange(resizedColumns);

            return newpage;
        }
        
        bool CheckBoundary(IEnumerable<IBlock> blockset, IBlock block)
        {
            foreach(var bl in blockset)
            {
                if (Block.HasOverlap(bl, block))
                    return false;
            }

            return true;
        }
        
    }
}
