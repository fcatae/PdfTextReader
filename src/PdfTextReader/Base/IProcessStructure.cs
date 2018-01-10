using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Base
{
    interface IProcessStructure<T> : ITransformStructure
    {
        T Process(T structure);
    }
}
