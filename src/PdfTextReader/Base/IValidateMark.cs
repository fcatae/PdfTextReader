using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Base
{
    interface IValidateMark
    {
        string Validate(BlockSet<MarkLine> marks);
    }
}
