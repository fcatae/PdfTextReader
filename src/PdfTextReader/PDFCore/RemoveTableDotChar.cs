using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class RemoveTableDotChar : IProcessBlock, IValidateBlock
    {
        public BlockPage Process(BlockPage page)
        {
            var result = new BlockPage();

            foreach (var block in page.AllBlocks)
            {
                if (block.GetText() != ".")
                    result.Add(block);
            }

            return result;
        }

        public BlockPage Validate(BlockPage page)
        {
            var result = new BlockPage();

            foreach(var block in page.AllBlocks)
            {
                if (block.GetText() == ".")
                    result.Add(block);
            }

            return result;
        }
    }
}
