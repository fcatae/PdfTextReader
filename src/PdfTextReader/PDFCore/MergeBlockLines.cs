using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class MergeBlockLines : IProcessBlock
    {
        public BlockPage Process(BlockPage page)
        {
            var result = new BlockPage();
            IBlock last = null;

            foreach (var block in page.AllBlocks)
            {
                var next = block;

                if( last != null )
                {
                    if((last.GetH() < block.GetH()) && Block.HasOverlap(last, block))
                    {
                        // merge
                        var blockset = last as BlockSet<IBlock>;

                        if( blockset == null )
                        {
                            var b = new BlockSet<IBlock>();
                            b.Add(last);
                            blockset = b;
                        }
                        
                        blockset.Add(block);
                        next = blockset;
                    }
                }

                result.Add(next);
                
                last = next;
            }

            return result;
        }
    }
}
