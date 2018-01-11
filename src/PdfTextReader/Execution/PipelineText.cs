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

        // public TextSet CurrentText;
        
        public void ReleaseAfterFinish(object instance)
        {
            var disposableObj = instance as IDisposable;
            if (disposableObj != null)
            {
                _disposableObjects.Add(disposableObj);
            }
        }

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

        T CreateInstance<T>()
            where T : new()
        {
            var obj = new T();

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
            return CreateNewPipelineText(PipelineTextLog<TL>(new StreamWriter(filename), this.CurrentStream, true));
        }

        public PipelineText<TT> Log<TL>(TextWriter writer)
            where TL : ILogStructure<TT>, new()
        {
            return CreateNewPipelineText(PipelineTextLog<TL>(writer, this.CurrentStream, false));
        }
        
        public PipelineText<TT> DebugCount(string message)
        {
            return CreateNewPipelineText(PipelineTextDebugCount(message, this.CurrentStream));
        }

        public PipelineText<TT> DebugPrint(string message)
        {
            return CreateNewPipelineText(PipelineTextDebugPrint(message, this.CurrentStream));
        }

        IEnumerable<TT> PipelineTextLog<TL>(TextWriter file, IEnumerable<TT> stream, Action<TextWriter> callbackDone)
            where TL : ILogStructure<TT>, new()
        {
            TL logger = default(TL);

            try
            {
                logger = CreateInstance<TL>();

                logger.StartLog(file);

                foreach (var data in stream)
                {
                    logger.Log(file, data);

                    yield return data;
                }

                logger.EndLog(file);
            }
            finally
            {
                FreeObject(logger);

                if(callbackDone!= null)
                {
                    callbackDone(file);
                }                
            }
        }

        IEnumerable<TT> PipelineTextLog<TL>(TextWriter file, IEnumerable<TT> stream, bool dispose)
            where TL : ILogStructure<TT>, new()
        {
            TL logger = default(TL);

            try
            {
                logger = CreateInstance<TL>();

                logger.StartLog(file);

                foreach (var data in stream)
                {
                    logger.Log(file, data);

                    yield return data;
                }

                logger.EndLog(file);
            }
            finally
            {
                FreeObject(logger);
                if (dispose)
                {
                    FreeObject(file);
                }
            }
        }

        IEnumerable<TT> PipelineTextDebugCount(string message, IEnumerable<TT> input)
        {
            int count = 0;
            foreach (var i in input)
            {
                count++;
                yield return i;
            }
            Console.WriteLine(message + ": " + count);
        }

        IEnumerable<TT> PipelineTextDebugPrint(string message, IEnumerable<TT> input)
        {
            foreach (var i in input)
            {
                Console.WriteLine(message + ": " + i.ToString());
                yield return i;
            }
        }
        
        //public IEnumerable<TT> ToEnumerable()
        //{
        //    // TODO: return a disposable 
        //    return CurrentStream;
        //}

        public IList<TT> ToList()
        {
            var result = CurrentStream.ToList();

            this.Dispose();

            return result;
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
