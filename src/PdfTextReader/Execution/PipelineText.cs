using PdfTextReader.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PdfTextReader.Execution
{
    class PipelineText
    {
        public IPipelineContext Context { get; }

        public PipelineText(IPipelineContext context, TextSet text)
        {
            this.Context = context;
            this.CurrentText = text;
        }

        public TextSet CurrentText;

        public PipelineText Debug(Color Color)
        {
            throw new NotImplementedException();
        }
        public PipelineText Show(Color Color)
        {
            PipelineDebug.Show((PipelineInputPdf)Context, CurrentText, Color);

            return this;
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
