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
using iText.Kernel.Pdf.Extgstate;
using System.IO;

namespace PdfTextReader.Execution
{
    class PipelineInputPdf : IPipelinePdfContext, IDisposable
    {
        private readonly string _input;
        private PdfDocument _pdfDocument;
        private string _output;
        private PdfDocument _pdfOutput;
        private List<object> _statsCollection = new List<object>();
        private List<List<object>> _statsCollectionPerPage = new List<List<object>>();
        private PipelinePdfLog _pdfLog = new PipelinePdfLog();
        private TransformIndexTree _indexTree = new TransformIndexTree();

        private static bool g_continueOnException = true;

        public static void StopOnException()
        {
            g_continueOnException = false;
        }
        public static PipelineInputPdf DebugCurrent;

        public PipelineInputPdfPage CurrentPage { get; private set; }
        public TransformIndexTree Index => _indexTree;

        public PipelineInputPdf(string filename)
        {
            var pdfDocument = new PdfDocument(VirtualFS.OpenPdfReader(filename));

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

            using (var pdfInput = new PdfDocument(VirtualFS.OpenPdfReader(_input)))
            {
                int total = pdfInput.GetNumberOfPages();
                var positivePages = Enumerable.Range(1, total).Except(errorPages).ToList();

                if (positivePages.Count == 0)
                    return;

                using (var pdfOutput = new PdfDocument(VirtualFS.OpenPdfWriter(outputfile)))
                {
                    pdfInput.CopyPagesTo(positivePages, pdfOutput);
                }
            }
        }

        
        public int ExtractOutputPages(string outputfile, IEnumerable<int> pages)
        {
            string inputfile = this._input;

            var pageList = pages.OrderBy(t => t).ToList();

            if (pageList.Count == 0)
                return 0;

            using (var pdfInput = new PdfDocument(VirtualFS.OpenPdfReader(_output)))
            using (var pdfOutput = new PdfDocument(VirtualFS.OpenPdfWriter(outputfile)))
            {
                pdfInput.CopyPagesTo(pageList, pdfOutput);
            }

            return pageList.Count;
        }

        public int SaveErrors(string outputfile)
        {
            string inputfile = this._input;

            var errorPages = _pdfLog.GetErrors().OrderBy(t=>t).ToList();

            if (errorPages.Count == 0)
                return 0;

            using (var pdfInput = new PdfDocument(VirtualFS.OpenPdfReader(_input)))
            using (var pdfOutput = new PdfDocument(VirtualFS.OpenPdfWriter(outputfile)))
            {
                pdfInput.CopyPagesTo(errorPages, pdfOutput);
            }

            return errorPages.Count;
        }

        public PipelineInputPdf Output(string outfile)
        {
            if( _pdfOutput != null )
            {
                ((IDisposable)_pdfOutput).Dispose();
            }

            var pdfOutput = new PdfDocument(VirtualFS.OpenPdfReader(_input), VirtualFS.OpenPdfWriter(outfile));

            this._output = outfile;
            this._pdfOutput = pdfOutput;

            return this;
        }

        public PipelineDebugContext CreatePipelineDebugContext(string outputname)
        {
            return new PipelineDebugContext(_input, outputname);
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

            using (var pdfInput = new PdfDocument(VirtualFS.OpenPdfReader(_input)) )
            using (var pdfOutput = new PdfDocument(VirtualFS.OpenPdfWriter(outfile)))
            {
                pdfInput.CopyPagesTo(pageNumbers, pdfOutput);                
            }
        }
        public void ExtractPages(string outfile, IList<int> pageNumbers)
        {
            using (var pdfInput = new PdfDocument(VirtualFS.OpenPdfReader(_input)))
            using (var pdfOutput = new PdfDocument(VirtualFS.OpenPdfWriter(outfile)))
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

            var pipeText = new PipelineText<TextLine>(this, textLines, _indexTree, this);

            return pipeText;
        }

        public PipelineText<TextLine> AllPages<T>(Action<PipelineInputPdfPage> callback)
            where T : IConvertBlock, new()
        {
            var textLines = StreamConvert<T>(callback);
            
            var pipeText = new PipelineText<TextLine>(this, textLines, _indexTree, this);
            
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

                if (ProtectCall(callback, pdfPage) == false)
                    continue;

                var textSet = processor.ProcessPage(i, CurrentPage.GetLastResult());

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

                if (ProtectCall(callback, pdfPage) == false)
                    continue;

                var textSet = processor.ProcessPage(i, CurrentPage.GetLastResult());

                foreach(var t in textSet)
                {
                    yield return t;
                }
            }
        }

        bool ProtectCall(Action<PipelineInputPdfPage> callback, PipelineInputPdfPage pdfPage)
        {
            if (!g_continueOnException)
            {
                callback(pdfPage);
                return true;
            }

            try
            {
                callback(pdfPage);
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                PipelineDebug.ShowException(this, ex);

                StoreStatistics(new StatsExceptionHandled(pdfPage.GetPageNumber(), ex));
                StoreStatistics(pdfPage.GetPageNumber(), new StatsExceptionHandled(pdfPage.GetPageNumber(), ex));
            }

            return false;
        }

        public void StoreStatistics(object stats)
        {
            _statsCollection.Add(stats);
        }

        public void StoreStatistics(int page, object stats)
        {
            int index = page - 1;

            if( index >= _statsCollectionPerPage.Count )
            {
                _statsCollectionPerPage.Add(new List<object>());
            }

            var stat = _statsCollectionPerPage[index];

            stat.Add(stats);
        }

        public IEnumerable<T> RetrieveStatistics<T>()
            where T: class
        {
            var availableStats = _statsCollection
                                    .Select(s => s as T)
                                    .Where(s => s != null);

            return availableStats;
        }

        public PipelineStats Statistics => new PipelineStats(_statsCollection, _statsCollectionPerPage);

        public class PipelineInputPdfPage : IDisposable
        {
            private readonly PipelineInputPdf _pdf;
            private readonly int _pageNumber;            
            private readonly PdfPage _pdfPage;
            private PipelinePage _page;
            private PdfCanvas _outputCanvas;

            private PipelineSingletonAutofacFactory _factory = new PipelineSingletonAutofacFactory();

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
            public void FillRectangle(double x, double h, double width, double height, System.Drawing.Color color)
            {
                var canvas = GetCanvas();

                var pdfColor = GetColor(color);

                int opacity = color.A;

                canvas.SaveState();
                if( opacity < 250 )
                {
                    PdfExtGState gstate = new PdfExtGState();
                    gstate.SetFillOpacity(color.A / 255f);
                    //gstate.SetBlendMode(PdfExtGState.BM_EXCLUSION);
                    canvas.SetExtGState(gstate);
                }                

                canvas.SetFillColor(pdfColor);
                canvas.Rectangle(x, h, width, height);
                canvas.Fill();
                canvas.RestoreState();
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

            public void DrawBackground(System.Drawing.Color color)
            {
                float page_width = _pdfPage.GetPageSize().GetWidth();
                float page_height = _pdfPage.GetPageSize().GetHeight();

                FillRectangle(0, 0, page_width, page_height, color);
            }

            public void DrawWarning(string message, float size, System.Drawing.Color color)
            {
                int MAXTEXTSIZE = (int)(1.2*_pdfPage.GetPageSize().GetHeight()/size);
                float margin = 10f;
                float linespace = size*1.15f;
                float paragraph = size * 2f;
                
                float x = margin;
                float h = _pdfPage.GetPageSize().GetHeight() - size - margin;
                
                string[] lines = message.Split('\n');

                foreach (var line in lines)
                {
                    string text;

                    for (text = line; text.Length > MAXTEXTSIZE; text = text.Substring(MAXTEXTSIZE))
                    {                        
                        DrawText(x, h, text.Substring(0, MAXTEXTSIZE), size, color);
                        h -= linespace;
                    }
                    DrawText(x, h, text, size, color);

                    h -= paragraph;
                }
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

                if( _factory != null )
                {
                    _factory.Dispose();
                    _factory = null;
                }
            }
        }        
    }
}
