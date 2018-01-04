using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using PdfTextReader.PDFCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Execution
{
    class PipelineInputPdf : IDisposable
    {
        private readonly string _input;
        private PdfDocument _pdfDocument;
        private string _output;
        private PdfDocument _pdfOutput;

        public PipelineInputPdf(string filename)
        {
            var pdfDocument = new PdfDocument(new PdfReader(filename));

            this._input = filename;
            this._pdfDocument = pdfDocument;
        }
        
        public PipelineInputPdfPage Page(int pageNumber)
        {
            return new PipelineInputPdfPage(this, pageNumber);
        }

        public PipelineInputPdf Output(string outfile)
        {
            if( _pdfOutput != null )
            {
                ((IDisposable)_pdfOutput).Dispose();
            }

            var pdfOutput = new PdfDocument(new PdfReader(_input), new PdfWriter(outfile));

            this._output = outfile;
            this._pdfOutput = pdfOutput;

            return this;
        }

        public void Dispose()
        {
            if (_pdfDocument != null)
            {
                ((IDisposable)_pdfDocument).Dispose();
                _pdfDocument = null;
            }

            if (_pdfOutput != null)
            {
                ((IDisposable)_pdfOutput).Dispose();
                _pdfOutput = null;
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
                where T: IEventListener, IPipelineResults<BlockPage>, new()
            {
                var listener = new T();

                var parser = new PdfCanvasProcessor(listener);

                parser.ProcessPageContent(_pdfPage);

                return new PipelinePage(_pdf, _pageNumber);
            }
        }        
    }
}
