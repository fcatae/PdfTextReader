using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.PDFCore
{
    class FindInitialBlocksetWithBlockInfo : IProcessBlock
    {
        private readonly BlockPage2 _blocksetInfo;

        // TODO: set it back to 100f or less
        private float statDownInTheBottom = 100f;
        private float statGoingUp = 5f;

        public FindInitialBlocksetWithBlockInfo(BlocksetData blocksetInfo)
        {
            this._blocksetInfo = blocksetInfo.Info;

            if (blocksetInfo.Info == null)
                PdfReaderException.AlwaysThrow("FindInitialBlocksetWithBlockInfo depends on BlocksetData");

            if (statDownInTheBottom > 1000f)
                System.Diagnostics.Debug.WriteLine($"Feature Disabled: {statDownInTheBottom}");
        }

        public BlockPage Process(BlockPage page)
        {
            IBlock last = null;
            BlockColumn lastColumn = null;
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

                var column = (BlockColumn)FindColumn(block);

                if (column == null)
                    PdfReaderException.Throw("Column not in the blockset info -- review stage 2");

                if(lastColumn != null)
                {
                    if (column != lastColumn)
                        shouldBreak = true;
                }

                if((currentBlockSet == null) || shouldBreak)
                {
                    currentBlockSet = new BlockSet<IBlock>();
                    result.Add(currentBlockSet);
                }                    

                currentBlockSet.Add(block);

                last = block;
                lastColumn = column;
            }

            return result;
        }

        IBlock FindColumn(IBlock block)
        {
            var columnList = _blocksetInfo.Segments.SelectMany(s => s.Columns);

            foreach (var column in columnList)
            {
                //bool contains = Block.Contains(column, block);
                bool contains = Block.HasOverlap(column, block);

                if (contains)
                    return column;
            }

            return null;
        }
    }
}
