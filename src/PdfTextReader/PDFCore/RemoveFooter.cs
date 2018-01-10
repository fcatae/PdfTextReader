using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.PDFCore
{
    class RemoveFooter : IProcessBlock, IValidateBlock
    {
        const float statRegionTooLarge = 200f;

        public BlockPage Process(BlockPage page)
        {
            float err = 1f;
            float minH = page.AllBlocks.Min(b => b.GetH()) + err;

            var blocksAtFooter = page.AllBlocks.Where(b => b.GetH() > minH);

            var result = new BlockPage();

            result.AddRange(blocksAtFooter);

            return result;
        }

        public BlockPage Validate(BlockPage page)
        {
            float err = 1f;
            float minH = page.AllBlocks.Min(b => b.GetH()) + err;

            var blocksAtFooter = page.AllBlocks.Where(b => b.GetH() <= minH);

            var result = new BlockPage();

            result.AddRange(blocksAtFooter);

            float height = result.AllBlocks.GetHeight();

            if (height > statRegionTooLarge)
                throw new InvalidOperationException();

            return result;
        }
    }
}
