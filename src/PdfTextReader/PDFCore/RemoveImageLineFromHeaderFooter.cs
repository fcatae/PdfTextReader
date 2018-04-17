using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace PdfTextReader.PDFCore
{
    class RemoveImageLineFromHeaderFooter : IProcessBlock
    {
        private float _headerH = float.NaN;
        private float _footerH = float.NaN;
        private ProcessImageData _imageData;
        private IdentifyTablesData _tablesData;

        public RemoveImageLineFromHeaderFooter(HeaderFooterData data, ProcessImageData imageData, IdentifyTablesData tablesData)
        {
            this._headerH = data.HeaderH;
            this._footerH = data.FooterH;
            this._imageData = imageData;
            this._tablesData = tablesData;

            if( float.IsNaN(_headerH) || float.IsNaN(_footerH) )
                PdfReaderException.AlwaysThrow("RemoveImageLineFromHeaderFooter requires HeaderFooterData");
        }

        public BlockPage Process(BlockPage page)
        {
            _imageData.Images = Filter(_imageData.Images);
            _tablesData.PageLines = Filter(_tablesData.PageLines);
            _tablesData.PageTables = Filter(_tablesData.PageTables);
            _tablesData.PageBackground = Filter(_tablesData.PageBackground);

            return Filter(page);
        }

        public BlockPage Filter(BlockPage page)
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
    }
}
