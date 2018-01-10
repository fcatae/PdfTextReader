using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Base
{
    interface IFilterStructure<T> : ITransformStructure
    {
        T Filter(T structure);
    }
}
