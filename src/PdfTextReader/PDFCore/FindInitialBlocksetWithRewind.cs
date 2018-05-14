using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.PDFCore
{
    class FindInitialBlocksetWithRewind : IProcessBlock
    {
        private float statDownInTheBottom = 100f;
        private float statGoingUp = 5f;

        public FindInitialBlocksetWithRewind()
        {
            if (statDownInTheBottom > 1000f)
            {
                System.Diagnostics.Debug.WriteLine($"Feature Disabled: {statDownInTheBottom}");
            }
        }

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

                    bool rewind = previous < next;
                    if ( (rewind) && ((Block)block).HasBackColor )
                    {
                        shouldBreak = true;
                    }

                }

                // check for superscript font
                if (( shouldBreak ) && (Block.IsSuperscriptFont((Block)last, (Block)block)))
                {
                    shouldBreak = false;
                }

                if (shouldBreak && currentBlockSet.Count()>1)
                {
                    var tableline = currentBlockSet.TakeLast(2).First();

                    if (Block.AreSameLine(tableline, block))
                        shouldBreak = false;
                }

                if ((currentBlockSet == null) || shouldBreak)
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
