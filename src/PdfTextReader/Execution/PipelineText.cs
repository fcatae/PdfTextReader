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
        private List<IDisposable> _disposableObjects;
        
        public PipelineText(IPipelineContext context, IEnumerable<TT> stream, IDisposable chain)
        {
            this.Context = context;
            this.CurrentStream = stream;
            this._disposableObjects = new List<IDisposable>() { chain };
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
            
            var processor = new TransformText<P,TT,TO>();
            ReleaseAfterFinish(processor);

            var result = processor.Transform(initial);

            var pipe = new PipelineText<TO>(this.Context, result, this);

            ((PipelineInputPdf)this.Context).SetCurrentText(pipe);

            return pipe;
        }

        PipelineText<TT> CreateNewPipelineText(IEnumerable<TT> stream)
        {
            return new PipelineText<TT>(this.Context, stream, this);
        }

        //public PipelineText<TT> Process<T>()
        //    where T : IProcessStructure<TT>, new()
        //{
        //    var processor = CreateInstance<T>();

        //    var stream = from data in ((IEnumerable<TT>)this.CurrentStream)
        //                 select processor.Process(data);

        //    return CreateNewPipelineText(stream);
        //}

        public PipelineText<TT> Log<TL>(string filename)
            where TL : ILogStructure<TT>, new()
        {
            var file = CreateInstance<TextWriter>( ()=> new StreamWriter(filename) );            

            return Log<TL>(file);
        }

        public PipelineText<TT> Log<TL>(TextWriter writer)
            where TL : ILogStructure<TT>, new()
        {
            return CreateNewPipelineText(PipelineTextLog<TL>(writer, this.CurrentStream));
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
            TL logger = CreateInstance<TL>();

            logger.StartLog(file);

            foreach (var data in stream)
            {
                logger.Log(file, data);

                yield return data;
            }

            logger.EndLog(file);
        }
        public void ReleaseAfterFinish(object instance)
        {
            var disposableObj = instance as IDisposable;
            if (disposableObj != null)
            {
                _disposableObjects.Add(disposableObj);
            }
        }

        T CreateInstance<T>()
            where T : new()
        {
            var obj = new T();

            ReleaseAfterFinish(obj);

            return obj;
        }

        T CreateInstance<T>(Func<T> create)
        {
            var obj = create();

            ReleaseAfterFinish(obj);

            return obj;
        }

        void FreeObject(object instance)
        {
            var disposable = instance as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        public void Dispose()
        {
            lock (_disposableObjects)
            {
                if (_disposableObjects != null)
                {
                    foreach (var obj in _disposableObjects)
                    {
                        if( obj != null )
                        {
                            obj.Dispose();
                        }
                    }

                    _disposableObjects = null;
                }
            }
        }
    }
}
