using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.PDFCore
{
    interface IValidateBlock
    {
        BlockPage Validate(BlockPage page);
    }
}
