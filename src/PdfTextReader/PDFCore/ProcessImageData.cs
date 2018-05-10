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
    class ProcessImageData : IProcessBlockData
    {
        private BlockSet<IBlock> _blockSet = new BlockSet<IBlock>();

        public BlockPage Images = null;

        public BlockPage LastResult { get; private set; }

        public void UpdateInstance(object cache)
        {
            var instance = (ProcessImageData)cache;
            this.Images = instance.Images;
            this.LastResult = instance.LastResult;
            this._blockSet = instance._blockSet;
        }

        public void RemoveImage(IBlock block)
        {
            if (!(block is ImageBlock))
                PdfReaderException.AlwaysThrow("Block is not ImageBlock");

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

            LastResult = newpage;

            return newpage;
        }
    }
}
