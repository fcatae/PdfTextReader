using PdfTextReader.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PdfTextReader.Execution
{
    class PipelineText<T>
    {
        public IPipelineContext Context { get; }
        public IEnumerable<T> CurrentStream;

        public PipelineText(IPipelineContext context, TextSet text)
        {
            this.Context = context;
            this.CurrentText = text;
        }
        public PipelineText(IPipelineContext context, IEnumerable<T> stream)
        {
            this.Context = context;
            this.CurrentStream = stream;
        }

        public TextSet CurrentText;

        public PipelineText<T> Debug(Color Color)
        {
            throw new NotImplementedException();
        }
        public PipelineText<T> Show(Color Color)
        {
            PipelineDebug.Show((PipelineInputPdf)Context, CurrentText, Color);

            return this;
        }
        public PipelineText<T> Validate<T>(Color Color)
        {
            throw new NotImplementedException();
        }

        public PipelineText<TextLine> ParseText<P>()
            where P: IProcessText, new()
        {
            var initial = this.CurrentText;

            var processor = new P();

            var result = processor.ProcessText(initial);

            this.CurrentText = result;
            this.CurrentStream = (IEnumerable<T>)result.AllText;

            return (PipelineText<TextLine>)((object)this);
        }
        public PipelineText<TO> ConvertText<T,TI,TO>()
            where T : class, ITransformStructure<TI,TO>, new()
        {
            var initial = (IEnumerable<TI>)this.CurrentStream;
            
            var processor = new TransformText<T,TI,TO>();

            var result = processor.Transform(initial);

            // this.CurrentStream = (IEnumerable<TO>)result;

            return new PipelineText<TO>(this.Context, result);
        }

        public PipelineText<T> ParseContent<T>()
        {
            throw new NotImplementedException();
        }
        
    }
}
