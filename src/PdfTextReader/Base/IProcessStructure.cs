using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Base
{
    interface IProcessStructure<T>
    {
        T Process(T structure);
    }
}
