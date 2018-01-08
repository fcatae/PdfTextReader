using PdfTextReader.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace PdfTextReader.Execution
{
    class PipelineText<TT>
    {
        public IPipelineContext Context { get; }
        public IEnumerable<TT> CurrentStream;

        public PipelineText(IPipelineContext context, TextSet text)
        {
            this.Context = context;
            this.CurrentText = text;
        }
        public PipelineText(IPipelineContext context, IEnumerable<TT> stream)
        {
            this.Context = context;
            this.CurrentStream = stream;
        }

        public TextSet CurrentText;

        public PipelineText<TT> Debug(Color Color)
        {
            throw new NotImplementedException();
        }
        public PipelineText<TT> Show(Color Color)
        {
            PipelineDebug.Show((PipelineInputPdf)Context, CurrentStream, Color);
            //PipelineDebug.Show((PipelineInputPdf)Context, CurrentText, Color);

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
            this.CurrentStream = (IEnumerable<TT>)result.AllText;

            return (PipelineText<TextLine>)((object)this);
        }
        public PipelineText<TO> ConvertText<P,TO>()
            where P : class, ITransformStructure<TT,TO>, new()
        {
            var initial = (IEnumerable<TT>)this.CurrentStream;
            
            var processor = new TransformText<P,TT,TO>();

            var result = processor.Transform(initial).ToList();

            var pipe = new PipelineText<TO>(this.Context, result);

            ((PipelineInputPdf)this.Context).CurrentText = pipe;

            return pipe;
        }

        public PipelineText<T> ParseContent<T>()
        {
            throw new NotImplementedException();
        }
        
    }
}
