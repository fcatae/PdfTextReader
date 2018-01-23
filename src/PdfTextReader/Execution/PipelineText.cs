using PdfTextReader.TextStructures;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using PdfTextReader.Base;
using System.IO;

namespace PdfTextReader.Execution
{
    class PipelineText<TT> : IDisposable
    {
        public IPipelineContext Context { get; }
        public IEnumerable<TT> CurrentStream;
        private PipelineFactory _factory = new PipelineFactory();
        private TransformIndexTree _indexTree;

        public PipelineText(IPipelineContext context, IEnumerable<TT> stream, TransformIndexTree indexTree, IDisposable chain)
        {
            this.Context = context;
            this.CurrentStream = stream;
            _factory = new PipelineFactory();
            _factory.AddReference(chain);
            _indexTree = indexTree;
        }
                
        public PipelineText<TT> Show(Color Color)
        {
            PipelineDebug.Show((PipelineInputPdf)Context, CurrentStream, Color);
            
            return this;
        }
                
        public PipelineText<TO> ConvertText<P,TO>()
            where P : class, IAggregateStructure<TT,TO>, new()
        {
            var initial = (IEnumerable<TT>)this.CurrentStream;
            
            var processor = _factory.CreateInstance( ()=> new TransformText<P,TT,TO>());

            var index = processor.GetIndexRef();
            _indexTree.AddRef(index);

            var result = processor.Transform(initial);

            var pipe = new PipelineText<TO>(this.Context, result, _indexTree, this);
            
            return pipe;
        }

        PipelineText<TT> CreateNewPipelineTextForLogging(IEnumerable<TT> stream)
        {
            return new PipelineText<TT>(this.Context, stream, _indexTree, this);
        }
        
        public PipelineText<TT> Log<TL>(string filename)
            where TL : ILogStructure<TT>, new()
        {
            var file = _factory.CreateInstance<TextWriter>( ()=> new StreamWriter(filename) );            

            return Log<TL>(file);
        }

        public PipelineText<TT> Log<TL>(TextWriter writer)
            where TL : ILogStructure<TT>, new()
        {
            return CreateNewPipelineTextForLogging(PipelineTextLog<TL>(writer, this.CurrentStream));
        }

        public IEnumerable<TT> ToEnumerable()
        {
            return ConvertPipelineTextToEnumerable(this);
        }

        public IList<TT> ToList()
        {
            var result = CurrentStream.ToList();

            this.Dispose();

            return result;
        }

        IEnumerable<TT> ConvertPipelineTextToEnumerable(PipelineText<TT> pipelineText)
        {
            var stream = pipelineText.CurrentStream;

            foreach (var data in stream)
            {
                yield return data;
            }

            pipelineText.Dispose();
        }

        IEnumerable<TT> PipelineTextLog<TL>(TextWriter file, IEnumerable<TT> stream)
            where TL : ILogStructure<TT>, new()
        {
            TL logger = _factory.CreateInstance<TL>();

            // v2: pass pipeline context
            var loggerV2 = logger as ILogStructure2<TT>;
            if( loggerV2 != null )
            {
                loggerV2.Init(_indexTree);
            }

            logger.StartLog(file);

            foreach (var data in stream)
            {
                logger.Log(file, data);

                yield return data;
            }

            logger.EndLog(file);
        }

        public void Dispose()
        {
            _factory.Dispose();
        }
    }
}
