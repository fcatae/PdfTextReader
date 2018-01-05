using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class BreakColumns : IProcessBlock, IValidateBlock
    {
        public BlockPage Process(BlockPage page)
        {
            return Validate(page);
        }

        public BlockPage Validate(BlockPage page)
        {
            var blocks = page.AllBlocks.ToList();
            var overlapped = new bool[blocks.Count];
            var result = new BlockPage();

            for(int i=0; i<blocks.Count; i++)
            {
                for(int j=i+1; j<blocks.Count; j++)
                {
                    if(Block.HasOverlap(blocks[i], blocks[j]))
                    {
                        overlapped[i] = true;
                        overlapped[j] = true;
                    }
                }

                if(overlapped[i])
                {
                    result.Add(blocks[i]);
                }
            }

            return result;
        }

    }
}
