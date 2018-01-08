using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Execution
{
    interface IPipelineDependency
    {
        void SetPage(PipelinePage page);
    }
}
