using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class RemoveHeader : IProcessBlock, IValidateBlock
    {
        const float statRegionTooLarge = 200f;

        public BlockPage Process(BlockPage page)
        {
            float err = 1f;
            float maxH = page.AllBlocks.Max(b => b.GetH()) - err;

            var blocksAtHeader = page.AllBlocks.Where(b => b.GetH() < maxH);

            var result = new BlockPage();

            result.AddRange(blocksAtHeader);

            return result;
        }

        public BlockPage Validate(BlockPage page)
        {
            float err = 1f;
            float maxH = page.AllBlocks.Max(b => b.GetH()) - err;

            var blocksAtHeader = page.AllBlocks.Where(b => b.GetH() >= maxH);

            var result = new BlockPage();

            result.AddRange(blocksAtHeader);

            float height = result.AllBlocks.GetHeight();
            if (height > statRegionTooLarge)
                throw new InvalidOperationException();

            return result;
        }
    }
}
