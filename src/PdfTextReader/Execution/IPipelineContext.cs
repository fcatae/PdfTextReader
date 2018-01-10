using System;
using System.Collections.Generic;
using System.Text;
using PdfTextReader.Base;

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
