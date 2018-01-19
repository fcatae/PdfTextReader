using PdfTextReader.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.PDFText;
using PdfTextReader.Base;

namespace PdfTextReader.PDFCore
{
    class RemoveTableOverImage : IProcessBlock, IPipelineDependency
    {
        private List<IBlock> _images;

        public void SetPage(PipelinePage p)
        {
            var parseImage = p.CreateInstance<PreProcessImages>();

            var page = parseImage.Images;

            if (page == null)
            {
                PdfReaderException.AlwaysThrow("RemoveTableOverImage requires PreProcessImages");
            }
            
            this._images = page.AllBlocks.ToList();
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
