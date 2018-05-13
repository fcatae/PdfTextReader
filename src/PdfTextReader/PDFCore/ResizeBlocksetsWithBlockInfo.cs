using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.PDFCore
{
    class ResizeBlocksetsWithBlockInfo : IProcessBlock
    {
        private readonly BlockPage2 _blocksetInfo;

        public ResizeBlocksetsWithBlockInfo(BlocksetData blocksetInfo)
        {
            this._blocksetInfo = blocksetInfo.Info;

            if (blocksetInfo.Info == null)
                PdfReaderException.AlwaysThrow("ResizeBlocksetsWithBlockInfo depends on BlocksetData");
        }

        public BlockPage Process(BlockPage page)
        {
            var result = new BlockPage();

            foreach(var block in page.AllBlocks)
            {
                var column = FindColumn(block);

                if (column == null)
                {
                    PdfReaderException.Warning("Invalid blockset column assigned -- review stage 2");
                    continue;
                }

                var bset = block as BlockSet<IBlock>;

                if( bset != null )
                {

                    var resizedBlock = new BlockSet2<IBlock>(bset, column.GetX(), bset.GetH(), column.GetX() + column.GetWidth(), bset.GetH() + bset.GetHeight());
                    result.Add(resizedBlock);
                }
                else
                {
                    // image or text?
                    result.Add(block);
                }
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
