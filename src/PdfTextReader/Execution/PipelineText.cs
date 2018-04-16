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
        private PipelineFactoryContext _pipelineFactory;

        public PipelineText(PipelineFactoryContext factory, IPipelineContext context, IEnumerable<TT> stream, TransformIndexTree indexTree, IDisposable chain)
        {
            this.Context = context;
            this.CurrentStream = stream;
            _factory = new PipelineFactory();
            _factory.AddReference(chain);
            _indexTree = indexTree;
            _pipelineFactory = factory;
        }

        PipelineInputPdf ParentContext => (PipelineInputPdf)this.Context;

        public PipelineText<TT> Show(Color Color)
        {
            PipelineDebug.Show((PipelineInputPdf)Context, CurrentStream, Color);
            
            return this;
        }
                
        public PipelineText<TO> ConvertText<P,TO>()
            where P : class, IAggregateStructure<TT,TO>
        {
            var initial = (IEnumerable<TT>)this.CurrentStream;
            var transform = _pipelineFactory.CreateGlobalInstance<P>();
            var processor = new TransformText<P,TT,TO>(transform);

            _factory.AddReference(processor);

            var index = processor.GetIndexRef();
            _indexTree.AddRef(index);

            var result = processor.Transform(initial);

            var pipe = new PipelineText<TO>(_pipelineFactory, this.Context, result, _indexTree, this);
            
            return pipe;
        }

        PipelineText<TT> CreateNewPipelineTextForLogging(IEnumerable<TT> stream)
        {
            return new PipelineText<TT>(this._pipelineFactory, this.Context, stream, _indexTree, this);
        }
        
        public PipelineText<TT> Log<TL>(string filename)
            where TL : class, ILogStructure<TT>
        {
            var file = VirtualFS.OpenStreamWriter(filename);
            _factory.AddReference(file);

            return Log<TL>(file);
        }
        public PipelineText<TT> ShowPdf<TL>(string filename)
            where TL : class, ILogStructurePdf<TT>
        {
            var pipeline = ParentContext.CreatePipelineDebugContext(filename);
            _factory.AddReference(pipeline);

            return ShowPdf<TL>(pipeline);
        }

        public PipelineText<TT> ShowPdf<TL>(IPipelineDebug pipelineDebug)
            where TL : class, ILogStructurePdf<TT>
        {
            return CreateNewPipelineTextForLogging(PipelineTextLogPdf<TL>(pipelineDebug, this.CurrentStream));
        }

        public PipelineText<TT> Log<TL>(TextWriter writer)
            where TL : class, ILogStructure<TT>
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
        IEnumerable<TT> PipelineTextLogPdf<TL>(IPipelineDebug pipelineDebug, IEnumerable<TT> stream)
            where TL : class, ILogStructurePdf<TT>
        {
            TL logger = _pipelineFactory.CreateInstance<TL>();
            
            logger.StartLogPdf(pipelineDebug);

            foreach (var data in stream)
            {
                logger.LogPdf(pipelineDebug, data);

                yield return data;
            }

            logger.EndLogPdf(pipelineDebug);
        }

        IEnumerable<TT> PipelineTextLog<TL>(TextWriter file, IEnumerable<TT> stream)
            where TL : class, ILogStructure<TT>
        {
            TL logger = _pipelineFactory.CreateInstance<TL>();
            //TL logger = _factory.CreateInstance<TL>();

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
