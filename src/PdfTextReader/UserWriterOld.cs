using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader
{
    class UserWriterOld
    {
        // blocklist (structure) - complex
        static void TryMergeBlockSets(List<BlockSet> blockList)
        {
            for (int i = 0; i < blockList.Count - 1; i++)
            {
                var curBlock = blockList[i];
                var nextBlock = blockList[i + 1];

                Console.WriteLine(curBlock.GetText());

                float a_x1 = curBlock.GetX();
                float a_x2 = curBlock.GetX() + curBlock.GetWidth();
                float b_x1 = nextBlock.GetX();
                float b_x2 = nextBlock.GetX() + nextBlock.GetWidth();

                // a contains b
                bool hasOverlapA = ((a_x1 <= b_x1) && (a_x2 >= b_x2));
                bool hasOverlapB = ((b_x1 <= a_x1) && (b_x2 >= a_x2));
                bool hasOverlap = hasOverlapA;// || hasOverlapB;

                if (hasOverlap)
                {
                    float a_y2 = curBlock.GetH() + curBlock.GetHeight();
                    float b_y2 = nextBlock.GetH() + nextBlock.GetHeight();

                    if (a_y2 > b_y2)
                    {

                    }
                    else
                    {

                    }

                    // Merge current and next blocks
                    var newblock = BlockSet.MergeBlocks(curBlock, nextBlock);

                    blockList[i] = null;
                    blockList[i + 1] = newblock;
                }
            }

            // remove nulled blocks
            blockList.RemoveAll(b => b == null);
        }

    }
}
