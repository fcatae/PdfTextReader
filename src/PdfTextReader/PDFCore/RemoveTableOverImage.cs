using PdfTextReader.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.PDFText;
using PdfTextReader.Base;

namespace PdfTextReader.PDFCore
{
    class RemoveTableOverImage : IProcessBlock
    {
        private List<IBlock> _images;
        private IBlock _headerImage;

        public RemoveTableOverImage(PreProcessImages parseImage, RemoveHeaderImage optionalHeaderImage)
        {
            var page = parseImage.Images;

            if (page == null)
            {
                PdfReaderException.AlwaysThrow("RemoveTableOverImage requires PreProcessImages");
            }

            this._images = page.AllBlocks.ToList();

            // optional is tricky - it comes from Dependency injection, so the optional is never null
            if (optionalHeaderImage != null && optionalHeaderImage.HeaderImage != null)
            {
                // add header image to the collection -- tables will be removed as well
                _headerImage = optionalHeaderImage.HeaderImage;

                _images.Add(_headerImage);
            }
        }

        public BlockPage Process(BlockPage page)
        {
            if (this._images == null)
            {
                PdfReaderException.AlwaysThrow("RemoveTableOverImage requires PreProcessImages");
            }

            var result = new BlockPage();

            foreach (var table in page.AllBlocks)
            {
                bool insideImage = false;

                if (table is TableSet)
                {
                    foreach (var img in _images)
                    {
                        if (Block.HasOverlap(img, table))
                        {
                            insideImage = true;
                            break;
                        }
                    }
                }

                if (!insideImage)
                {
                    result.Add(table);
                }
            }

            return result;
        }
    }
}
