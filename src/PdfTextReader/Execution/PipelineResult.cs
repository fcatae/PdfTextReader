using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Execution
{
    interface IPipelineResults<T>
    {
        T GetResults();
    }
}
