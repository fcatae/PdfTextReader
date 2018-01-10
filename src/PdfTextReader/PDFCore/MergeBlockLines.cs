using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.PDFCore
{
    class MergeBlockLines : IProcessBlock
    {
        public BlockPage Process(BlockPage page)
        {
            var result = new BlockPage();
            BlockSet<IBlock> last = null;

            foreach (var block in page.AllBlocks)
            {
                var blockset = (BlockSet<IBlock>)block;

                if ((last == null) || (!CanBeMerged(last, blockset)))
                {
                    var b = new BlockSet<IBlock>();
                    b.AddRange(blockset);

                    result.Add(b);

                    last = b;
                }
                else
                {
                    // merge blocks
                    last.AddRange(blockset);
                }
            }

            return result;
        }

        bool CanBeMerged(BlockSet<IBlock> a, BlockSet<IBlock> b)
        {
            var lastLine = a.Last();
            var firstLine = b.First();
            
            return Block.HasOverlap(lastLine, firstLine);
        }
    }
}
