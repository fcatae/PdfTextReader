using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Base
{
    interface ICalculateStats<T>
    {
        object Calculate(IEnumerable<T> stats);
    }
}
