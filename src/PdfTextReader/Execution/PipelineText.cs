using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PdfTextReader.Execution
{
    class PipelineText
    {
        public PipelineText Output(string filename)
        {
            throw new NotImplementedException();
        }
        public PipelineText Show(Color Color)
        {
            throw new NotImplementedException();
        }
        public PipelineText Debug(Color Color)
        {
            throw new NotImplementedException();
        }
        public PipelineText Show<T>(Func<T, bool> filter, Color Color)
        {
            throw new NotImplementedException();
        }
        public PipelineText Validate<T>(Color Color)
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
