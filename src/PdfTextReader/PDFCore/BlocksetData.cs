using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class BlocksetData : IProcessBlockData
    {
        private BlockPage2 _blocksetInfo;

        public BlockPage2 Info => _blocksetInfo;
        public BlockPage LastResult => _blocksetInfo;

        public BlockPage Process(BlockPage page2)
        {
            var page = page2 as BlockPage2;

            if (page == null)
                PdfReaderException.AlwaysThrow("BlocksetData must execute AFTER OrganizePageLayout");

            var blocksetInfo = new BlockPage2();

            foreach (var segment in page.Segments)
            {
                var segmentInfo = new BlockPageSegment(blocksetInfo, segment.NumberOfColumns);

                foreach (var column in segment.Columns)
                {
                    var columnInfo = CopyColumnMetadata(blocksetInfo, column);

                    segmentInfo.AddColumn(columnInfo);
                }

                blocksetInfo.AddSegment(segmentInfo);
            }

            this._blocksetInfo = blocksetInfo;

            return page;
        }

        BlockColumn CopyColumnMetadata(BlockPage2 blocksetInfo, BlockColumn column)
        {
            var columnInfo = new BlockColumn(blocksetInfo, column.ColumnType, column.X1, column.W);

            var block = new Block()
            {
                H = column.GetH(),
                X = column.GetX(),
                Height = column.GetHeight(),
                Width = column.GetWidth(),
                Text = $"Column [{columnInfo.ColumnType}:{columnInfo.X1}]"
            };
            var bset = new BlockSet<IBlock>(blocksetInfo);
            bset.Add(block);

            columnInfo.Add(bset);

            return columnInfo;
        }

        public void UpdateInstance(object cache)
        {
            var instance = (BlocksetData)cache;
            this._blocksetInfo = instance._blocksetInfo;
        }
    }
}
