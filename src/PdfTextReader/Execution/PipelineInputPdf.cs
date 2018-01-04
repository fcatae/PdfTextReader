using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using PdfTextReader.PDFCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Execution
{
    class PipelineInputPdf : IPipelinePdfContext, IDisposable
    {
        private readonly string _input;
        private PdfDocument _pdfDocument;
        private string _output;
        private PdfDocument _pdfOutput;
        
        public PipelineInputPdfPage CurrentPage { get; private set; }

        public PipelineInputPdf(string filename)
        {
            var pdfDocument = new PdfDocument(new PdfReader(filename));

            this._input = filename;
            this._pdfDocument = pdfDocument;
        }
        
        public PipelineInputPdfPage Page(int pageNumber)
        {
            var page = new PipelineInputPdfPage(this, pageNumber);

            if( CurrentPage != null )
            {
                CurrentPage.Dispose();
            }

            CurrentPage = page;

            return page;
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
            if( CurrentPage != null )
            {
                CurrentPage.Dispose();
                CurrentPage = null;
            }

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

        public class PipelineInputPdfPage : IDisposable
        {
            private readonly PipelineInputPdf _pdf;
            private readonly int _pageNumber;            
            private readonly PdfPage _pdfPage;
            private PdfCanvas _outputCanvas;

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

                var page = new PipelinePage(_pdf, _pageNumber);
                page.LastResult = listener.GetResults();

                if (page.LastResult == null)
                    throw new InvalidOperationException();

                if (page.LastResult.AllBlocks == null)
                    throw new InvalidOperationException();

                return page;
            }

            public PipelineInputPdfPage Output(string filename)
            {
                this._pdf.Output(filename);
                return this;
            }

            PdfCanvas GetCanvas()
            {
                if(_outputCanvas == null)
                {
                    var page = _pdf._pdfOutput.GetPage(_pageNumber);                                        
                    var canvas = new PdfCanvas(page);

                    _outputCanvas = canvas;
                }                

                return _outputCanvas;
            }

            iText.Kernel.Colors.DeviceRgb GetColor(System.Drawing.Color color)
            {
                return new iText.Kernel.Colors.DeviceRgb(color.R, color.G, color.B);
            }

            public void DrawRectangle(double x, double h, double width, double height, System.Drawing.Color color)
            {
                var canvas = GetCanvas();

                var pdfColor = GetColor(color);

                canvas.SetStrokeColor(pdfColor);
                canvas.Rectangle(x, h, width, height);
                canvas.Stroke();
            }
            public void DrawLine(double x1, double h1, double x2, double h2, System.Drawing.Color color)
            {
                var canvas = GetCanvas();

                var pdfColor = GetColor(color);

                canvas.SetStrokeColor(pdfColor);
                canvas.MoveTo(x1, h1);
                canvas.LineTo(x2, h2);
                canvas.Stroke();
            }
            

            public void Dispose()
            {
                if( _outputCanvas != null )
                {
                    _outputCanvas.Release();
                    _outputCanvas = null;
                }
            }
        }        
    }
}
