using PdfTextReader.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.PDFText;
using PdfTextReader.Base;

namespace PdfTextReader.PDFCore
{
    class RemoveHeaderImage : IProcessBlock, IValidateBlock
    {
        const float statRegionTooLarge = 200f;
        private List<IBlock> _images;
        private PreProcessImages _parse;
        private IBlock _headerImage = null;

        public IBlock HeaderImage => _headerImage;

        public RemoveHeaderImage(PreProcessImages parseImage)
        {
            var page = parseImage.Images;

            if (page == null)
            {
                PdfReaderException.AlwaysThrow("RemoveHeaderImage requires PreProcessImages");
            }

            this._images = page.AllBlocks.ToList();

            this._parse = parseImage;
        }

        public BlockPage Process(BlockPage page)
        {
            if (this._images == null)
            {
                PdfReaderException.AlwaysThrow("RemoveHeaderImage requires PreProcessImages");
            }

            var topImage = FindTopImage(this._images);

            _headerImage = topImage;

            if (topImage == null)
                PdfReaderException.AlwaysThrow("image == null");

            var result = RemoveHeaderImageAndAbove(page, topImage);

            this._parse.RemoveImage(topImage);

            return result;
        }

        public IBlock FindTopImage(IEnumerable<IBlock> images)
        {
            return _images.OrderByDescending(t => t.GetH()).FirstOrDefault();
        }

        public BlockPage RemoveHeaderImageAndAbove(BlockPage page, IBlock image)
        {
            var result = new BlockPage();

            float imageH = image.GetH();
            bool foundHeader = false;

            foreach (var block in page.AllBlocks)
            {
                float h = block.GetH() + block.GetHeight();

                if (h > imageH)
                {
                    if (block.GetHeight() > statRegionTooLarge)
                        PdfReaderException.Throw("block.GetHeight() > statRegionTooLarge");

                    foundHeader = true;
                    continue;
                }

                result.Add(block);
            }

            bool checkFailure = (foundHeader == false) || (imageH < 500f);

            if(checkFailure)
                PdfReaderException.Throw("(foundHeader == false) || (imageH < 500f)");

            return result;
        }

        public BlockPage Validate(BlockPage page)
        {
            var result = new BlockPage();

            if (this._images == null)
                PdfReaderException.AlwaysThrow("RemoveHeaderImage requires PreProcessImages");

            var topImage = FindTopImage(this._images);
            
            if (topImage != null)
            {
                result.Add(topImage);
            }

            return result;
        }
    }
}
