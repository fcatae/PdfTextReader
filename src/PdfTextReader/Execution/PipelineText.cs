using PdfTextReader.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PdfTextReader.Execution
{
    class PipelineText
    {
        TextSet CurrentText;

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
            where T: IProcessText, new()
        {
            var initial = this.CurrentText;

            var processor = new T();

            var result = processor.ProcessText(initial);

            this.CurrentText = result;

            return this;
        }

        public PipelineText ParseContent<T>()
        {
            throw new NotImplementedException();
        }
        
    }
}
