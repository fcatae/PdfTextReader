using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using PdfTextReader.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.PDFCore
{
    class ProcessImageData : IProcessBlock
    {
        private BlockSet<IBlock> _blockSet = new BlockSet<IBlock>();

        public BlockPage Images = null;

        public void RemoveImage(IBlock block)
        {
            if (Images == null)
                PdfReaderException.AlwaysThrow("Images == null");

            int before = Images.AllBlocks.Count();

            var allBlocksMinusOne = Images.AllBlocks.Except(new IBlock[] { block });

            Images = new BlockPage();
            Images.AddRange(allBlocksMinusOne);

            int after = Images.AllBlocks.Count();

            if (after == before)
                PdfReaderException.AlwaysThrow("after == before");
        }

        public BlockPage Process(BlockPage page)
        {
            var newpage = new BlockPage();

            newpage.AddRange(page.AllBlocks.AsEnumerable());

            this.Images = newpage;

            return newpage;
        }
    }
}
