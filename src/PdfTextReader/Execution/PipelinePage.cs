using iText.Kernel.Pdf.Canvas.Parser.Listener;
using PdfTextReader.PDFCore;
using PdfTextReader.TextStructures;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.Execution
{
    class PipelinePage
    {
        public IPipelinePdfContext Context { get; }
        public int PageNumber { get; }
        private BlockPage LastErrors { get; set; }

        public BlockPage LastResult { get; set; }

        public PipelinePage(PipelineInputPdf pdf, int pageNumber)
        {
            this.Context = pdf;
            this.PageNumber = pageNumber;
        }

        PipelineInputPdf ParentContext => (PipelineInputPdf) this.Context;

        public PipelinePage Debug(Color Color)
        {
            throw new NotImplementedException();
        }
        public PipelinePage Show<T>(Func<T, bool> filter, Color Color)
        {
            throw new NotImplementedException();
        }
        public PipelinePage Validate<T>(Color Color)
            where T: IValidateBlock, new()
        {
            var initial = this.LastResult;

            var processor = new T();

            var result = processor.Validate(initial);

            this.LastErrors = result;

            return this;
        }
        public PipelinePage PdfCheck<T>(Color Color)
            where T : IValidateMark, new()
        {
            if (Color != Color.Orange)
                throw new InvalidOperationException();

            int color = MarkLine.ORANGE;

            var initialMarks = this.LastResult.AllBlocks.Cast<MarkLine>().Where(l => l.Color == color);
            var marks = new BlockSet<MarkLine>();
            marks.AddRange(initialMarks);

            var processor = new T();

            string message = processor.Validate(marks);

            if( message != null)
            {
                ParentContext.LogCheck(PageNumber, typeof(T), message);
            }

            return this;
        }
        public PipelinePage Validate<T>(Action<BlockSet<IBlock>> filter = null)
            where T : class, IValidateBlock
        {
            var initial = this.LastResult;

            var processor = CreateInstance<T>();

            var result = processor.Validate(initial);
            
            if( filter != null && result != null )
            {
                //foreach(var blockSet in )
                filter(result.AllBlocks);
            }

            this.LastErrors = result;

            return this;
        }
        
        public T CreateInstance<T>() where T : class
        {
            var obj = ((PipelineInputPdf)Context).CurrentPage.CreateInstance<T>();

            return obj;
        }
        
        public PipelinePage ParseBlock<T>()
            where T: class, IProcessBlock
        {
            var initial = this.LastResult;
            
            var processor = CreateInstance<T>();

            var result = processor.Process(initial);

            // Get result
            if (result == null)
                throw new InvalidOperationException();

            // Get statistics
            var stats = processor as IRetrieveStatistics;
            if (stats != null)
            {
                CollectStatistics(stats);
            }

            int beforeCount = this.LastResult.AllBlocks.Count();

            this.LastResult = result;

            if (result.IsEmpty() && beforeCount > 0)
                PdfReaderException.Warning($"{typeof(T).Name} returned no data");

            return this;
        }

        public PipelinePage PrintWarnings()
        {
            var warnings = PdfReaderException.GetPageWarnings();

            if(warnings.Count() > 0)
            {
                PipelineDebug.ShowWarnings(this.ParentContext, warnings);
            }
            
            return this;
        }

        void CollectStatistics(IRetrieveStatistics process)
        {
            if (process == null)
                throw new ArgumentNullException();

            var stats = process.RetrieveStatistics();

            this.ParentContext.StoreStatistics(stats);
            this.ParentContext.StoreStatistics(PageNumber, stats);
        }

        public PipelinePage ShowErrors(Action<PipelinePage> callback)
        {
            var newpage = new PipelinePage((PipelineInputPdf)this.Context, this.PageNumber);

            var errors = this.LastErrors;

            newpage.LastResult = errors;
            newpage.LastErrors = errors;

            callback(newpage);
                        
            return this;
        }
        
        public PipelinePage ParsePdf<T>()
            where T : class, IEventListener, IPipelineResults<BlockPage>, new()
        {
            return ((PipelineInputPdf)this.Context).CurrentPage.ParsePdf<T>();
        }

        public PipelinePage StoreCache<T>()
            where T : class, IProcessBlockData
        {
            var processor = CreateInstance<T>();

            processor.Process(this.LastResult);

            this.Context.StoreCache<T>(PageNumber, processor);
            return this;
        }

        public PipelinePage FromCache<T>()
            where T : class, IProcessBlockData
        {
            var processor = CreateInstance<T>();

            var cache = this.Context.FromCache<T>(PageNumber);

            processor.UpdateInstance(cache);

            var result = cache.LastResult;

            this.LastResult = result;
            return this;
        }
    }
}
