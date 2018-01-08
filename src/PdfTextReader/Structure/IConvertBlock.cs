using PdfTextReader.PDFCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Structure
{
    interface IConvertBlock
    {
        TextSet ConvertBlock(BlockPage page);
    }
}
