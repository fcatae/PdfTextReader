using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class RemoveSmallFonts : IProcessBlock, IValidateBlock
    {
        const float CONSIDERED_SMALL_FONTSIZE = 1f;

        public BlockPage Process(BlockPage page)
        {
            var result = new BlockPage();

            foreach(var block in page.AllBlocks)
            {
                if( ((Block)block).FontSize > CONSIDERED_SMALL_FONTSIZE )
                {
                    result.Add(block);
                }
            }

            return result;
        }

        public BlockPage Validate(BlockPage page)
        {
            var result = new BlockPage();

            foreach (var block in page.AllBlocks)
            {
                if (((Block)block).FontSize <= CONSIDERED_SMALL_FONTSIZE)
                {
                    float boxSize = 10f;

                    var box = new Block()
                    {
                        X = block.GetX() - boxSize,
                        H = block.GetH() - boxSize,
                        Width = block.GetWidth() + 2*boxSize,
                        Height = block.GetHeight() + 2*boxSize
                    };
                    result.Add(box);
                }
            }

            return result;
        }
    }
}
