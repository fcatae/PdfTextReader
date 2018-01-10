using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Base
{
    interface IConvertBlock
    {
        TextSet ConvertBlock(BlockPage page);
    }
}
