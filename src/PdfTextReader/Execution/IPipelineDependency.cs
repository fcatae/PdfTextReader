using System;
using System.Collections.Generic;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.Execution
{
    [Obsolete]
    interface IPipelineDependency
    {
        void SetPage(PipelinePage page);
    }
}
