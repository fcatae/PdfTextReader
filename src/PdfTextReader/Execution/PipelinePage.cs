using PdfTextReader.PDFCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PdfTextReader.Execution
{
    class PipelinePage
    {
        public PipelinePage(PipelineInputPdf pdf, int pageNumber)
        {
            this.Context = pdf;
            this.PageNumber = pageNumber;
        }

        public PipelineInputPdf Context { get; }
        public int PageNumber { get; }
        public BlockPage LastResult { get; set; }

        public PipelinePage Debug(Color Color)
        {
            throw new NotImplementedException();
        }
        public PipelinePage Show<T>(Func<T, bool> filter, Color Color)
        {
            throw new NotImplementedException();
        }
        public PipelinePage Validate<T>(Color Color)
        {
            throw new NotImplementedException();
        }
        public PipelinePage Validate<T>(Action<object> action)
        {
            throw new NotImplementedException();
        }

        public PipelinePage ParseBlock<T>()
        {
            var data = this.LastResult;

            throw new NotImplementedException();
        }
        public PipelineText Text<T>()
        {
            throw new NotImplementedException();
        }

        public PipelinePage ParsePdf<T>()
        {
            // should be extension method
            throw new NotImplementedException();
        }

    }
}
