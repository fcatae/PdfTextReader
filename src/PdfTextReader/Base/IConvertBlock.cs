using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Base
{
    interface IConvertBlock
    {
        IEnumerable<TextLine> ProcessPage(BlockPage page);
    }
}
