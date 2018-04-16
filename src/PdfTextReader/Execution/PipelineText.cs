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
        private PipelineDisposeHelper _tracker = new PipelineDisposeHelper();
        private TransformIndexTree _indexTree;
        private PipelineFactory _factory;

        public PipelineText(PipelineFactory factory, IPipelineContext context, IEnumerable<TT> stream, TransformIndexTree indexTree, IDisposable chain)
        {
            this.Context = context;
            this.CurrentStream = stream;
            _tracker = new PipelineDisposeHelper();
            _tracker.TrackInstance(chain);
            _indexTree = indexTree;
            _factory = factory;
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
            var transform = _factory.CreateGlobalInstance<P>();
            var processor = new TransformText<P,TT,TO>(transform);

            _tracker.TrackInstance(processor);

            var index = processor.GetIndexRef();
            _indexTree.AddRef(index);

            var result = processor.Transform(initial);

            var pipe = new PipelineText<TO>(_factory, this.Context, result, _indexTree, this);
            
            return pipe;
        }

        PipelineText<TT> CreateNewPipelineTextForLogging(IEnumerable<TT> stream)
        {
            return new PipelineText<TT>(this._factory, this.Context, stream, _indexTree, this);
        }
        
        public PipelineText<TT> Log<TL>(string filename)
            where TL : class, ILogStructure<TT>
        {
            var file = VirtualFS.OpenStreamWriter(filename);
            _tracker.TrackInstance(file);

            return Log<TL>(file);
        }
        public PipelineText<TT> ShowPdf<TL>(string filename)
            where TL : class, ILogStructurePdf<TT>
        {
            var pipeline = ParentContext.CreatePipelineDebugContext(filename);
            _tracker.TrackInstance(pipeline);

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
            TL logger = _factory.CreateInstance<TL>();
            
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
            TL logger = _factory.CreateInstance<TL>();
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
            _tracker.Dispose();
        }
    }
}
