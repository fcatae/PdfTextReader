using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Base
{
    interface IProcessBlock
    {
        BlockPage Process(BlockPage page);
    }
}
