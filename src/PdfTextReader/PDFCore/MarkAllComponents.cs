using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.PDFCore
{
    class MarkAllComponents : IProcessBlock
    {
        public BlockPage Process(BlockPage page)
        {
            return page;
        }
    }
}
