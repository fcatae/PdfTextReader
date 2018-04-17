using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class ShowHeaderFooter : IProcessBlock, IValidateBlock
    {
        private float _headerH = float.NaN;
        private float _footerH = float.NaN;

        public ShowHeaderFooter(HeaderFooterData data)
        {
            _headerH = data.HeaderH;
            _footerH = data.FooterH;

            if (float.IsNaN(_headerH) || float.IsNaN(_footerH))
                PdfReaderException.AlwaysThrow("ShowHeaderFooter requires HeaderFooterData");
        }

        public BlockPage Process(BlockPage page)
        {
            var headerFooter = new BlockPage();

            AddBlockSet(headerFooter, page, b => b.GetH() >= _headerH);
            AddBlockSet(headerFooter, page, b => b.GetH() <= _footerH);

            return headerFooter;
        }

        public BlockPage Validate(BlockPage page)
        {
            var content = new BlockPage();

            AddBlockSet(content, page, b => b.GetH() < _headerH && b.GetH() > _footerH );

            return content;
        }

        void AddBlockSet(BlockPage dest, BlockPage source, Func<IBlock, bool> filter)
        {
            var blockset = GroupBy(source, filter);

            if (blockset != null)
                dest.Add(blockset);
        }

        BlockSet<IBlock> GroupBy(BlockPage page, Func<IBlock, bool> filter)
        {
            var blocks = page.AllBlocks.Where(filter);

            var blockset = new BlockSet<IBlock>(page);
            blockset.AddRange(blocks);

            if (blocks.Count() == 0)
                return null;

            return blockset;
        }
    }
}
