using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Base
{
    interface IValidateBlock
    {
        BlockPage Validate(BlockPage page);
    }
}
