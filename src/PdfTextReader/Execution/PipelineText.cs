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

        public PipelineText<TT> Process<T>()
            where T : IProcessStructure<TT>, new()
        {
            var processor = CreateInstance<T>();

            var stream = from data in ((IEnumerable<TT>)this.CurrentStream)
                         select processor.Process(data);

            return CreateNewPipelineText(stream);
        }

        public PipelineText<TT> Log<TL>(string filename)
            where TL : ILogStructure<TT>, new()
        {
            return CreateNewPipelineText(Log<TL>(filename, this.CurrentStream));
        }

        public PipelineText<TT> Log<TL>(TextWriter writer)
            where TL : ILogStructure<TT>, new()
        {
            return CreateNewPipelineText(Log<TL>(writer, this.CurrentStream));
        }

        IEnumerable<TT> Log<TL>(TextWriter writer, IEnumerable<TT> stream)
            where TL : ILogStructure<TT>, new()
        {
            var logger = CreateInstance<TL>();

            using (var file = writer)
            {
                logger.StartLog(file);

                foreach (var data in stream)
                {
                    logger.Log(file, data);

                    yield return data;
                }

                logger.EndLog(file);
            }
        }

        IEnumerable<TT> Log<TL>(string filename, IEnumerable<TT> stream)
            where TL : ILogStructure<TT>, new()
        {
            var logger = CreateInstance<TL>();

            using (var file = new StreamWriter(filename))
            {
                logger.StartLog(file);

                foreach(var data in stream)
                {
                    logger.Log(file, data);

                    yield return data;
                }

                logger.EndLog(file);
            }
        }

        T CreateInstance<T>()
            where T: new()
        {
            var obj = new T();

            ReleaseAfterFinish(obj);

            return obj;
        }

        PipelineText<TT> CreateNewPipelineText(IEnumerable<TT> stream)
        {
            return new PipelineText<TT>(this.Context, stream, this);
        }

        public PipelineText<TT> Process(IProcessStructure<TT> processor, bool dispose = true)
        {
            if(dispose)
            {
                ReleaseAfterFinish(processor);
            }

            var initial = (IEnumerable<TT>)this.CurrentStream;

            var result = initial.Select( data => processor.Process( data ));

            var pipe = new PipelineText<TT>(this.Context, result, this);
            
            return pipe;
        }

        public PipelineText<TT> Process2(IProcessStructure2<TT> processor)
        {
            ReleaseAfterFinish(processor);

            var initial = (IEnumerable<TT>)this.CurrentStream;

            var result = processor.Process(initial);

            var pipe = new PipelineText<TT>(this.Context, result, this);

            return pipe;
        }
        
        public PipelineText<TT> DebugCount(string message)
        {
            var pipe = new PipelineText<TT>(this.Context, DebugCount(CurrentStream), this);
            
            IEnumerable<TT> DebugCount(IEnumerable<TT> input)
            {
                int count = 0;
                foreach (var i in input)
                {
                    count++;
                    yield return i;
                }
                Console.WriteLine(message + ": " + count);
            }

            return pipe;
        }

        private PipelineText<TT> SaveFile(string filename)
        {
            IEnumerable<TT> SaveFileFunc(IEnumerable<TT> input)
            {
                foreach (var i in input)
                {
                    Console.WriteLine(filename + ": " + i.ToString());
                    yield return i;
                }
            }

            return new PipelineText<TT>(this.Context, SaveFileFunc(CurrentStream), this); ;
        }


        public PipelineText<TT> DebugPrint(string message)
        {
            var pipe = new PipelineText<TT>(this.Context, DebugPrint(CurrentStream), this);

            IEnumerable<TT> DebugPrint(IEnumerable<TT> input)
            {
                foreach (var i in input)
                {
                    Console.WriteLine(message + ": " + i.ToString());
                    yield return i;
                }                
            }

            return pipe;
        }

        public IEnumerable<TT> ToEnumerable()
        {
            // TODO: return a disposable 
            return CurrentStream;
        }

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
