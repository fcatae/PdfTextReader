using System;
using System.Collections.Generic;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.PDFCore
{
    class FindInitialBlockset : IProcessBlock
    {
        // TODO: set it back to 100f or less
        private float statDownInTheBottom = 100f;
        private float statGoingUp = 5f;

        public FindInitialBlockset()
        {
            if (statDownInTheBottom > 1000f)
                System.Diagnostics.Debug.WriteLine($"Feature Disabled: {statDownInTheBottom}");
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
