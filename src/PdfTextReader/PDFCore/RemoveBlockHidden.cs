using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class RemoveBlockHidden : IProcessBlock, IValidateBlock
    {
        public BlockPage Process(BlockPage page)
        {
            var newpage = new BlockPage();

            foreach(var block in page.AllBlocks)
            {
                if (block is BlockHidden)
                    continue;

                newpage.Add(block);
            }

            return newpage;
        }

        public BlockPage Validate(BlockPage page)
        {
            var newpage = new BlockPage();

            foreach (var block in page.AllBlocks)
            {
                if (block is BlockHidden)
                {
                    newpage.Add(block);
                }
            }

            return newpage;
        }
    }
}
