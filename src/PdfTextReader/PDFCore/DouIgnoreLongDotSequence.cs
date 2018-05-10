using System;
using System.Collections.Generic;
using System.Text;
using PdfTextReader.Base;
using System.Linq;

namespace PdfTextReader.PDFCore
{
    // bug #37: "omisses" can span multiple columns
    // proposed fix: decrease the width
    // should be run after GroupLines, to prevent cases where omisses are broken
    class DouIgnoreLongDotSequence : IProcessBlock
    {        
        public BlockPage Process(BlockPage page)
        {
            var result = new BlockPage();

            foreach (var block in page.AllBlocks)
            {
                var blockLine = (BlockLine)block;

                // divide by 4
                if(blockLine.GetText().Contains("...................."))
                {
                    blockLine.Width /= 4;
                }

                result.Add(blockLine);
            }

            return result;
        }
    }
}
