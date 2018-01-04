using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Execution
{
    class PipelinePage
    {
        public PipelinePage StoreResult(string name)
        {
            throw new NotImplementedException();
        }
        public PipelinePage Output(string filename)
        {
            throw new NotImplementedException();
        }
        public PipelinePage Show(System.Drawing.Color Color)
        {
            throw new NotImplementedException();
        }
        public PipelinePage Debug(System.Drawing.Color Color)
        {
            throw new NotImplementedException();
        }
        public PipelinePage Show<T>(Func<T, bool> filter, System.Drawing.Color Color)
        {
            throw new NotImplementedException();
        }
        public PipelinePage Validate<T>(System.Drawing.Color Color)
        {
            throw new NotImplementedException();
        }
        public PipelinePage Validate<T>(Action<object> action)
        {
            throw new NotImplementedException();
        }

        public PipelinePage ParseBlock<T>()
        {
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
