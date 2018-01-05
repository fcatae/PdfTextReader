using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class FindInitialBlockset : IProcessBlock
    {
        const float statDownInTheBottom = 100f;
        const float statGoingUp = 5f;

        public BlockPage Process(BlockPage page)
        {
            IBlock last = null;
            BlockSet<IBlock> currentBlockSet = null;
            var result = new BlockPage();

            foreach(var block in page.AllBlocks)
            { 
                bool shouldBreak = false;

                if (last != null)
                {
                    // expect: previous >~ next
                    float previous = last.GetH();
                    float next = block.GetH();

                    // previous >> next
                    if (previous > next + statDownInTheBottom)
                    {
                        shouldBreak = true;
                    }

                    // previous < next
                    if (previous < next - statGoingUp)
                    {
                        shouldBreak = true;
                    }
                }
                
                if((currentBlockSet == null) || shouldBreak)
                {
                    currentBlockSet = new BlockSet<IBlock>();
                    result.Add(currentBlockSet);
                }                    

                currentBlockSet.Add(block);

                last = block;
            }

            return result;
        }
    }
}
