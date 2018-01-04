using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Execution
{
    interface IPipelineContext
    {
    }
    interface IPipelinePdfContext : IPipelineContext
    {
        PipelineInputPdf.PipelineInputPdfPage CurrentPage { get; }
    }
}
