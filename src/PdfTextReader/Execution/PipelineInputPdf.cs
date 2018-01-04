using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Execution
{
    class PipelineInputPdf : IDisposable
    {
        private readonly string _filename;
        private PdfDocument _pdfDocument;

        public PipelineInputPdf(string filename)
        {
            var pdfDocument = new PdfDocument(new PdfReader(filename));

            this._filename = filename;
            this._pdfDocument = pdfDocument;
        }
        
        public PipelineInputPdfPage Page(int pageNumber)
        {
            return new PipelineInputPdfPage(this, pageNumber);
        }

        public void Dispose()
        {
            if( _pdfDocument != null )
            {
                ((IDisposable)_pdfDocument).Dispose();
                _pdfDocument = null;
            }
        }

        public class PipelineInputPdfPage
        {
            private readonly PipelineInputPdf _pdf;
            private readonly int _pageNumber;
            private PdfPage _pdfPage;

            public PipelineInputPdfPage(PipelineInputPdf pipelineInputContext, int pageNumber)
            {
                var pdfPage = pipelineInputContext._pdfDocument.GetPage(pageNumber);

                this._pdf = pipelineInputContext;
                this._pageNumber = pageNumber;
                this._pdfPage = pdfPage;
            }

            public PipelinePage ParsePdf<T>()
                where T: IEventListener, new()
            {
                var listener = new T();

                var parser = new PdfCanvasProcessor(listener);

                parser.ProcessPageContent(_pdfPage);

                return new PipelinePage();
            }
        }        
    }
}
