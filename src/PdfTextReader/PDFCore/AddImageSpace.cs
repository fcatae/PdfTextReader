using PdfTextReader.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.PDFText;

namespace PdfTextReader.PDFCore
{
    class AddImageSpace : IProcessBlock, IPipelineDependency
    {
        private List<IBlock> _images;

        public void SetPage(PipelinePage p)
        {
            var parserImage = p.CreateInstance<PreProcessImages>();

            var page = parserImage.Images;

            if (page == null)
                throw new InvalidOperationException("AddImageSpace requires PreProcessImages");

            this._images = page.AllBlocks.ToList();
        }

        public BlockPage Process(BlockPage page)
        {
            if(this._images == null)
                throw new InvalidOperationException("AddImageSpace requires PreProcessImages");

            var result = new BlockPage();

            foreach (var block in page.AllBlocks)
            {
                result.Add(block);
            }
            foreach (var block in _images)
            {
                result.Add(block);
            }

            return result;
        }
    }
}
