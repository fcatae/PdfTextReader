using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Execution
{
    class PipelineText
    {
        public PipelineText Output(string filename)
        {
            throw new NotImplementedException();
        }
        public PipelineText Show(System.Drawing.Color Color)
        {
            throw new NotImplementedException();
        }
        public PipelineText Debug(System.Drawing.Color Color)
        {
            throw new NotImplementedException();
        }
        public PipelineText Show<T>(Func<T, bool> filter, System.Drawing.Color Color)
        {
            throw new NotImplementedException();
        }
        public PipelineText Validate<T>(System.Drawing.Color Color)
        {
            throw new NotImplementedException();
        }
        public PipelineText ParseText<T>()
        {
            throw new NotImplementedException();
        }
        public PipelineText ParseContent<T>()
        {
            throw new NotImplementedException();
        }
        
    }
}
