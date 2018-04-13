using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Base
{
    interface IProcessBlockData : IProcessBlock
    {
        BlockPage LastResult { get; }
    }
}
