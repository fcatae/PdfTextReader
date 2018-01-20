using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using PdfTextReader.PDFCore;
using PdfTextReader.TextStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;
using iText.Kernel.Font;
using iText.IO.Font.Constants;

namespace PdfTextReader.Execution
{
    class PipelineInputPdf : IPipelinePdfContext, IDisposable
    {
        private readonly string _input;
        private PdfDocument _pdfDocument;
        private string _output;
        private PdfDocument _pdfOutput;
        private List<object> _statsCollection = new List<object>();
        private PipelinePdfLog _pdfLog = new PipelinePdfLog();

        public static PipelineInputPdf DebugCurrent;

        public PipelineInputPdfPage CurrentPage { get; private set; }
                
        public PipelineInputPdf(string filename)
        {
            var pdfDocument = new PdfDocument(new PdfReader(filename));

            this._input = filename;
            this._pdfDocument = pdfDocument;

            PipelineInputPdf.DebugCurrent = this;

            PdfReaderException.ClearContext();
        }
        
        public PipelineInputPdfPage Page(int pageNumber)
        {
            if (CurrentPage != null)
            {
                CurrentPage.Dispose();
            }

            var page = new PipelineInputPdfPage(this, pageNumber);
            
            CurrentPage = page;

            return page;
        }

        public void LogCheck(int pageNumber, Type component, string message)
        {
            _pdfLog.LogCheck(pageNumber, component, message);
        }

        public void SaveOk(string outputfile)
        {
            string inputfile = this._input;

            var errorPages = _pdfLog.GetErrors().OrderBy(t => t).ToList();            

            using (var pdfInput = new PdfDocument(new PdfReader(_input)))
            {
                int total = pdfInput.GetNumberOfPages();
                var positivePages = Enumerable.Range(1, total).Except(errorPages).ToList();

                if (positivePages.Count == 0)
                    return;

                using (var pdfOutput = new PdfDocument(new PdfWriter(outputfile)))
                {
                    pdfInput.CopyPagesTo(positivePages, pdfOutput);
                }
            }
        }

        public void SaveErrors(string outputfile)
        {
            string inputfile = this._input;

            var errorPages = _pdfLog.GetErrors().OrderBy(t=>t).ToList();

            if (errorPages.Count == 0)
                return;

            using (var pdfInput = new PdfDocument(new PdfReader(_input)))
            using (var pdfOutput = new PdfDocument(new PdfWriter(outputfile)))
            {
                pdfInput.CopyPagesTo(errorPages, pdfOutput);
            }
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

        public void Extract(string outfile, int start, int end)
        {
            IList<int> pageNumbers = Enumerable.Range(start, end - start + 1).ToList();

            using (var pdfInput = new PdfDocument(new PdfReader(_input)) )
            using (var pdfOutput = new PdfDocument(new PdfWriter(outfile)))
            {
                pdfInput.CopyPagesTo(pageNumbers, pdfOutput);                
            }
        }

        public void AllPages(Action<PipelineInputPdfPage> callback)
        {
            int totalPages = _pdfDocument.GetNumberOfPages();

            for (int i=1; i<=totalPages; i++)
            {
                var pdfPage = Page(i);

                callback(pdfPage);
            }
        }

        public PipelineText<TextLine> AllPagesExcept<T>(IEnumerable<int> exceptPages, Action<PipelineInputPdfPage> callback)
            where T : IConvertBlock, new()
        {
            var pageList = Enumerable.Range(1, _pdfDocument.GetNumberOfPages()).Except(exceptPages);

            var textLines = StreamConvert<T>(pageList, callback);

            var pipeText = new PipelineText<TextLine>(this, textLines, this);

            return pipeText;
        }

        public PipelineText<TextLine> AllPages<T>(Action<PipelineInputPdfPage> callback)
            where T : IConvertBlock, new()
        {
            var textLines = StreamConvert<T>(callback);
            
            var pipeText = new PipelineText<TextLine>(this, textLines, this);
            
            return pipeText;
        }

        public IEnumerable<TextLine> StreamConvert<T>(IEnumerable<int> pageList, Action<PipelineInputPdfPage> callback)
            where T : IConvertBlock, new()
        {
            var processor = new T();

            int totalPages = _pdfDocument.GetNumberOfPages();

            foreach (int i in pageList)
            {
                System.Diagnostics.Debug.WriteLine("Processing page " + i);

                var pdfPage = Page(i);

                callback(pdfPage);

                var textSet = processor.ProcessPage(CurrentPage.GetLastResult());

                foreach (var t in textSet)
                {
                    yield return t;
                }
            }
        }

        public IEnumerable<TextLine> StreamConvert<T>(Action<PipelineInputPdfPage> callback)
            where T: IConvertBlock, new()
        {
            var processor = new T();            

            int totalPages = _pdfDocument.GetNumberOfPages();

            for (int i = 1; i <= totalPages; i++)
            {
                var pdfPage = Page(i);

                callback(pdfPage);

                var textSet = processor.ProcessPage(CurrentPage.GetLastResult());

                foreach(var t in textSet)
                {
                    yield return t;
                }
            }
        }

        public void StoreStatistics(object stats)
        {
            _statsCollection.Add(stats);
        }

        public IEnumerable<T> RetrieveStatistics<T>()
            where T: class
        {
            var availableStats = _statsCollection
                                    .Select(s => s as T)
                                    .Where(s => s != null);

            return availableStats;
        }

        public PipelineStats Statistics => new PipelineStats(_statsCollection);

        public class PipelineInputPdfPage : IDisposable
        {
            private readonly PipelineInputPdf _pdf;
            private readonly int _pageNumber;            
            private readonly PdfPage _pdfPage;
            private PipelinePage _page;
            private PdfCanvas _outputCanvas;

            private PipelineSingletonFactory _factory = new PipelineSingletonFactory();

            public int GetPageNumber() => _pageNumber;
            public BlockPage GetLastResult() => _page.LastResult;

            public PipelineInputPdfPage(PipelineInputPdf pipelineInputContext, int pageNumber)
            {
                var pdfPage = pipelineInputContext._pdfDocument.GetPage(pageNumber);

                this._pdf = pipelineInputContext;
                this._pageNumber = pageNumber;
                this._pdfPage = pdfPage;

                PdfReaderException.SetContext(_pdf._input, pageNumber);
            }

            public T CreateInstance<T>()
                where T: new()
            {
                return _factory.CreateInstance<T>();
            }

            public PipelinePage ParsePdf<T>()
                where T: IEventListener, IPipelineResults<BlockPage>, new()
            {
                var listener = CreateInstance<T>();

                var parser = new PdfCanvasProcessor(listener);
                parser.ProcessPageContent(_pdfPage);

                var page = new PipelinePage(_pdf,  _pageNumber);

                page.LastResult = listener.GetResults();

                if (page.LastResult == null)
                    throw new InvalidOperationException();

                if (page.LastResult.AllBlocks == null)
                    throw new InvalidOperationException();

                _page = page;

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
            public void DrawText(double x, double h, string text, float size, System.Drawing.Color color)
            {
                var canvas = GetCanvas();

                var pdfColor = GetColor(color);
                
                canvas.SetColor(pdfColor, true);
                //canvas.Rectangle(x, h, width, height);
                canvas.BeginText();
                canvas.MoveText(x, h);
                var font = PdfFontFactory.CreateFont(StandardFonts.COURIER);
                canvas.SetFontAndSize(font, size);
                canvas.ShowText(text);
                canvas.EndText();
                //canvas.Stroke();
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
                PdfReaderException.ClearContext();

                if ( _outputCanvas != null )
                {
                    _outputCanvas.Release();
                    _outputCanvas = null;
                }
            }
        }        
    }
}
