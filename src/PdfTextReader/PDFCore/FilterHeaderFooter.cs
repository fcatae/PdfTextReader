using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace PdfTextReader.PDFCore
{
    class FilterHeaderFooter : IProcessBlock, IValidateBlock
    {
        private readonly BlockPage _lines;
        private readonly BlockPage _images;
        private float _headerH = float.NaN;
        private float _footerH = float.NaN;

        public FilterHeaderFooter(HeaderFooterData data)
        {
            _headerH = data.HeaderH;
            _footerH = data.FooterH;

            if( float.IsNaN(_headerH) || float.IsNaN(_footerH) )
                PdfReaderException.AlwaysThrow("FilterHeaderFooter requires HeaderFooterData");
        }

        public BlockPage Process(BlockPage page)
        {
            var content = new BlockPage();

            foreach(var b in page.AllBlocks)
            {
                if( b.GetH() > _footerH && b.GetH() < _headerH )
                {
                    content.Add(b);
                }
            }

            return content;
        }

        public BlockPage Validate(BlockPage page)
        {
            var headerfooter = new BlockPage();

            foreach (var b in page.AllBlocks)
            {
                if (b.GetH() <= _footerH || b.GetH() >= _headerH)
                {
                    headerfooter.Add(b);
                }
            }

            return headerfooter;
        }
    }
}
