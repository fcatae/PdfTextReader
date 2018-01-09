using PdfTextReader.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class RemoveImageTexts : IProcessBlock, IPipelineDependency
    {
        private List<IBlock> _images;

        public void SetPage(PipelinePage p)
        {
            var parseImage = p.CreateInstance<PDFCore.PreProcessImages>();

            var page = parseImage.Images;

            if (page == null)
                throw new InvalidOperationException("RemoveImageTexts requires PreProcessImages");

            this._images = page.AllBlocks.ToList();
        }

        public BlockPage Process(BlockPage page)
        {
            if (this._images == null)
                throw new InvalidOperationException("RemoveImageTexts requires PreProcessImages");

            var result = new BlockPage();

            foreach (var block in page.AllBlocks)
            {
                bool insideImage = false;

                foreach (var table in _images)
                {
                    if (Block.HasOverlap(table, block))
                    {
                        insideImage = true;
                        break;
                    }
                }

                if (!insideImage)
                {
                    result.Add(block);
                }
            }

            return result;
        }
    }
}
