using iText.Kernel.Pdf.Canvas.Parser.Listener;
using PdfTextReader.PDFCore;
using PdfTextReader.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace PdfTextReader.Execution
{
    class PipelinePage
    {
        public IPipelineContext Context { get; }
        public int PageNumber { get; }
        private BlockPage LastErrors { get; set; }

        public BlockPage LastResult { get; set; }

        public PipelinePage(PipelineInputPdf pdf, int pageNumber)
        {
            this.Context = pdf;
            this.PageNumber = pageNumber;
        }

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

        public PipelinePage Validate<T>(Action<BlockSet<IBlock>> filter = null)
            where T : IValidateBlock, new()
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
        
        public T CreateInstance<T>()
            where T : new()
        {
            var obj = ((PipelineInputPdf)Context).CurrentPage.CreateInstance<T>();

            var deps = obj as IPipelineDependency;
            if(deps != null)
            {
                deps.SetPage(this);
            }

            return obj;
        }

        public PipelinePage ParseBlock<T>()
            where T: IProcessBlock, new()
        {
            var initial = this.LastResult;
            
            var processor = CreateInstance<T>();

            var result = processor.Process(initial);

            if (result == null)
                throw new InvalidOperationException();

            this.LastResult = result;

            if (result.AllBlocks.Count() == 0)
                Console.WriteLine($"Warning: {typeof(T).Name} returned no data");

            return this;
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

        public PipelineText<TextLine> Text<T>()
            where T: IConvertBlock, new()
        {
            var proc2 = new T();
            var lines = proc2.ConvertBlock(this.LastResult);

            var pipe = new PipelineText<TextLine>(Context, lines);

            pipe.CurrentStream = lines.AllText;

            // ((PipelineInputPdf)this.Context).CurrentText = pipe;

            return pipe;
        }

        public PipelinePage ParsePdf<T>()
            where T : IEventListener, IPipelineResults<BlockPage>, new()
        {
            return ((PipelineInputPdf)this.Context).CurrentPage.ParsePdf<T>();
        }

    }
}
