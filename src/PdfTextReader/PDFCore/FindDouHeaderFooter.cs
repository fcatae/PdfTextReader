using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace PdfTextReader.PDFCore
{
    class FindDouHeaderFooter : IProcessBlock, IValidateBlock
    {
        private readonly BlockPage _lines;
        private readonly BlockPage _images;
        private float _headerH = float.NaN;
        private float _footerH = float.NaN;

        public FindDouHeaderFooter(IdentifyTablesData tables, ProcessImageData images)
        {
            this._lines = tables.PageLines;
            this._images = images.Images;

            if (_lines == null || _images == null)
                PdfReaderException.AlwaysThrow("FindDouHeaderFooter requires both IdentifyTablesData and ProcessImageData");
        }

        public BlockPage Process(BlockPage page)
        {
            FindMargins();

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
            FindMargins();

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

        void FindMargins()
        {
            var header = FindTopImage(_images.AllBlocks);
            var footer = FindBottomLine(_lines.AllBlocks);

            if (header != null)
            {
                this._headerH = header.GetH();
            }
            else
            {
                this._headerH = float.MaxValue;
                PdfReaderException.Warning("There is no image defining the header");
            }

            if( footer != null )
            {
                this._footerH = footer.GetH();
            }
            else
            {
                this._footerH = float.MinValue;
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
            return lines.OrderBy(t => t.GetH()).FirstOrDefault();
        }
    }
}
