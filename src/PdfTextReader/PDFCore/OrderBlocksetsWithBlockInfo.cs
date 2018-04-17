using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.PDFCore
{
    class OrderBlocksetsWithBlockInfo : IProcessBlock
    {
        private readonly BlockPage2 _blocksetInfo;

        public OrderBlocksetsWithBlockInfo(BlocksetData blocksetInfo)
        {
            this._blocksetInfo = blocksetInfo.Info;

            if (blocksetInfo.Info == null)
                PdfReaderException.AlwaysThrow("OrderBlocksetsWithBlockInfo depends on BlocksetData");
        }

        public BlockPage Process(BlockPage page)
        {
            var result = new BlockPage();

            var columnSequence = page.AllBlocks.Select(block =>
            {
                int columnId = FindColumnId(block);

                if (columnId < 0)
                    PdfReaderException.AlwaysThrow("Invalid blockset column assigned -- review stage 2 and 3");

                return new ColumnSequence
                {
                    ColumnId = columnId,
                    H = block.GetH(),
                    Block = block
                };
            })
            .OrderBy(block => block);

            var dbg = columnSequence.ToArray();

            result.AddRange(columnSequence.Select(b => b.Block));

            return result;
        }

        int FindColumnId(IBlock block)
        {
            var columnList = _blocksetInfo.Segments.SelectMany(s => s.Columns).Cast<BlockColumn>();

            int columnId = 0;
            foreach (var column in columnList)
            {
                bool contains = Block.HasOverlap(column, block);

                if (contains)
                    return columnId;

                columnId++;
            }

            return -1;
        }

        class ColumnSequence : IComparable
        {
            public int ColumnId;
            public float H;
            public IBlock Block;

            public int CompareTo(object obj)
            {
                var instance = (ColumnSequence)obj;

                if( instance.ColumnId == this.ColumnId )
                {
                    // return the inverse
                    return -H.CompareTo(instance.H);
                }

                return ColumnId.CompareTo(instance.ColumnId);
            }
        }
    }
}
