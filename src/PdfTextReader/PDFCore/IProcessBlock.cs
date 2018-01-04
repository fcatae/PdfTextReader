using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.PDFCore
{
    interface IProcessBlock
    {
        BlockPage Process(BlockPage page);
    }
}
