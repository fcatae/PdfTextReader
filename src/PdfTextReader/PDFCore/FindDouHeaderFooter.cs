using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace PdfTextReader.PDFCore
{
    class FindDouHeaderFooter : IProcessBlock
    {
        private readonly HeaderFooterData _headerFooterData;
        private readonly BlockPage _lines;
        private readonly BlockPage _images;

        public FindDouHeaderFooter(IdentifyTablesData tables, ProcessImageData images, HeaderFooterData headerFooterData)
        {
            this._headerFooterData = headerFooterData;
            this._lines = tables.PageLines;
            this._images = images.Images;

            if (_lines == null || _images == null)
                PdfReaderException.AlwaysThrow("FindDouHeaderFooter requires both IdentifyTablesData and ProcessImageData");
        }

        public BlockPage Process(BlockPage page)
        {
            FindMargins(_images.AllBlocks, _lines.AllBlocks, _headerFooterData);

            return page;
        }

        void FindMargins(IEnumerable<IBlock> images, IEnumerable<IBlock> lines, HeaderFooterData headerFooterData)
        {
            var header = FindTopImage(images);
            var footer = FindBottomLine(lines);

            if (header != null)
            {
                headerFooterData.HeaderH = header.GetH();
            }
            else
            {
                headerFooterData.HeaderH = float.MaxValue;
                PdfReaderException.Warning("There is no image defining the header");
            }

            if( footer != null )
            {
                headerFooterData.FooterH = footer.GetH();
            }
            else
            {
                headerFooterData.FooterH = float.MinValue;
                PdfReaderException.Warning("There is no (table) line defining the footer");
            }
        }

        // copied from RemoveHeaderImage
        public IBlock FindTopImage(IEnumerable<IBlock> images)
        {
            return images.OrderByDescending(t => t.GetH()).FirstOrDefault();
        }

        public IBlock FindBottomLine(IEnumerable<IBlock> lines)
        {
            return lines.Where(t => t.GetWidth() > 500f).OrderBy(t => t.GetH()).FirstOrDefault();
        }
    }
}
