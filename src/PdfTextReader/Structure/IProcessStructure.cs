using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Structure
{
    interface IProcessStructure<I, O>
    {
        O ConvertStructure(IEnumerable<I> input);
    }
}
