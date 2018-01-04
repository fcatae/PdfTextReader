using PdfTextReader.PDFCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PdfTextReader.Execution
{
    class PipelinePage
    {
        public PipelineInputPdf Context { get; }
        public int PageNumber { get; }
        public BlockPage LastResult { get; set; }
        private BlockPage LastErrors { get; set; }

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

        public PipelinePage Validate<T>(Action<BlockSet<Block>> filter = null)
            where T : IValidateBlock, new()
        {
            var initial = this.LastResult;

            var processor = new T();

            var result = processor.Validate(initial);

            if( filter != null && result != null )
            {
                //foreach(var blockSet in )
                filter(result.Current);
            }

            this.LastErrors = result;

            return this;
        }

        public PipelinePage ParseBlock<T>()
            where T: IProcessBlock, new()
        {
            var initial = this.LastResult;

            var processor = new T();

            var result = processor.Process(initial);

            if (result == null)
                throw new InvalidOperationException();

            this.LastResult = result;
            
            return this;
        }

        public PipelineText Text<T>()
        {
            throw new NotImplementedException();
        }

        public PipelinePage ParsePdf<T>()
        {
            // should be extension method
            throw new NotImplementedException();
        }

    }
}
