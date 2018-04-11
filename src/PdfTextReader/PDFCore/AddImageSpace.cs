using PdfTextReader.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.PDFText;
using PdfTextReader.Base;

namespace PdfTextReader.PDFCore
{
    class AddImageSpace : IProcessBlock
    {
        private List<IBlock> _images;

        public AddImageSpace(PreProcessImages parserImage)
        {
            var page = parserImage.Images;

            if (page == null)
            {
                PdfReaderException.AlwaysThrow("AddImageSpace requires PreProcessImages");
            }

            this._images = page.AllBlocks.ToList();
        }

        public BlockPage Process(BlockPage page)
        {
            if(this._images == null)
            {
                PdfReaderException.AlwaysThrow("AddImageSpace requires PreProcessImages");
            }

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
