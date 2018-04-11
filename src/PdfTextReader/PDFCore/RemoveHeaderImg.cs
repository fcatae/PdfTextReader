using PdfTextReader.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.PDFText;
using PdfTextReader.Base;

namespace PdfTextReader.PDFCore
{
    class RemoveHeaderImg : IProcessBlock, IValidateBlock //, IPipelineDependency
    {
        const float statRegionTooLarge = 200f;
        private List<IBlock> _images;
        private PreProcessImages _parse;
        private IBlock _headerImage = null;

        public IBlock HeaderImage => _headerImage;

        public RemoveHeaderImg(PreProcessImages parseImage)
        {
            var page = parseImage.Images;

            if (page == null)
            {
                PdfReaderException.AlwaysThrow("RemoveHeaderImage requires PreProcessImages");
            }

            this._images = page.AllBlocks.ToList();

            this._parse = parseImage;
        }

        //public void SetPage(PipelinePage p)
        //{
        //    var parseImage = p.CreateInstance<PreProcessImages>();

        //    var page = parseImage.Images;

        //    if (page == null)
        //    {
        //        PdfReaderException.AlwaysThrow("RemoveHeaderImage requires PreProcessImages");
        //    }

        //    this._images = page.AllBlocks.ToList();

        //    this._parse = parseImage;
        //}

        public BlockPage Process(BlockPage page)
        {
            if (this._images == null)
            {
                PdfReaderException.AlwaysThrow("RemoveHeaderImage requires PreProcessImages");
            }

            var header = FindBlocksAtHeader(page);

            var image = FindImageOverlap(header);

            _headerImage = image;

            if (image == null)
                PdfReaderException.AlwaysThrow("image == null");

            var result = RemoveHeaderImageWithText(page, image);

            this._parse.RemoveImage(image);

            return result;
        }

        public BlockPage FindBlocksAtHeader(BlockPage page)
        {
            float err = 1f;
            float maxH = page.AllBlocks.Max(b => b.GetH()) - err;

            var blocksAtHeader = page.AllBlocks.Where(b => b.GetH() >= maxH);

            var result = new BlockPage();

            result.AddRange(blocksAtHeader);

            return result;
        }               

        public IBlock FindImageOverlap(BlockPage page)
        {
            var result = new BlockPage();

            foreach (var block in page.AllBlocks)
            {
                foreach (var table in _images)
                {
                    if (Block.HasOverlap(table, block))
                    {
                        return table;
                    }
                }
            }

            return null;
        }

        public BlockPage RemoveHeaderImageWithText(BlockPage page, IBlock table)
        {
            if (this._images == null)
                PdfReaderException.AlwaysThrow("RemoveImageTexts requires PreProcessImages");

            var result = new BlockPage();

            foreach (var block in page.AllBlocks)
            {
                if (!Block.HasOverlap(table, block))
                {
                    result.Add(block);
                }
            }

            return result;
        }

        public BlockPage Validate(BlockPage page)
        {
            if (this._images == null)
                PdfReaderException.AlwaysThrow("RemoveHeaderImage requires PreProcessImages");

            var header = FindBlocksAtHeader(page);

            var image = FindImageOverlap(header);

            var result = new BlockPage();

            if (image != null)
            {
                result.Add(image);
            }

            return result;
        }
    }
}
