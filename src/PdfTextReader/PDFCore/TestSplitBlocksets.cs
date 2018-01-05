using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class TestSplitBlocksets : IProcessBlock
    {
        public BlockPage Process(BlockPage page)
        {
            var result = new BlockPage();

            foreach(var block in page.AllBlocks)
            {
                var bs = Split((BlockSet<IBlock>)block);

                if (bs != null)
                {
                    result.Add(bs[0]);
                    result.Add(bs[1]);
                }
                else
                {
                    result.Add(block);
                }
            }

            return result;
        }

        BlockSet<IBlock>[] Split(BlockSet<IBlock> blockset)
        {
            var list = blockset.ToList();
            int count = list.Count;

            if (count <= 1)
                return null;

            var l1 = list.Take(count / 2);
            var l2 = list.TakeLast(count - count / 2);

            var bl1 = new BlockSet<IBlock>();
            var bl2 = new BlockSet<IBlock>();

            bl1.AddRange(l1);
            bl2.AddRange(l2);

            return new BlockSet<IBlock>[] { bl1, bl2 };
        }
    }
}
