using System;
using System.Collections.Generic;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.Execution
{
    interface IPipelineResults<T>
    {
        T GetResults();
    }
}
