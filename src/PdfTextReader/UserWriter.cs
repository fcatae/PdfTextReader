using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Layout;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PdfTextReader
{
    class UserWriter
    {
        public void Process(string srcpath, string dstpath)
        {
            using (var pdf = new PdfDocument(new PdfReader(srcpath), new PdfWriter(dstpath)))
            {
                //var document = new Document(pdf);
                var page = pdf.GetPage(1);

                var canvas = new PdfCanvas(page);

                canvas.Rectangle(10, 10, 100, 100);
                canvas.Stroke();
            }
        }

        public void Process(string srcpath, Action<Block> func)
        {
            var parser = new PdfCanvasProcessor(new UserListener(func));

            using (var pdf = new PdfDocument(new PdfReader(srcpath)))
            {
                var page = pdf.GetPage(1);

                parser.ProcessPageContent(page);
            }
        }

        public void ProcessMarker(string srcpath, string dstpath)
        {
            using (var pdf = new PdfDocument(new PdfReader(srcpath), new PdfWriter(dstpath)))
            {
                var page = pdf.GetPage(1);
                var canvas = new PdfCanvas(page);
                var blockSet = new BlockSet();
                Block last = null;

                var parser = new PdfCanvasProcessor(new UserListener( b => {

                    bool shouldBreak = false;
                    
                    if( last != null )
                    {
                        string txt1 = last.Text;
                        string txt2 = b.Text;

                        float previous = last.H;
                        float next = b.H;

                        // expect: previous >~ next

                        // previous >> next
                        if( previous > next + 100)
                        {
                            shouldBreak = true;
                        }

                        // previous < next
                        if( previous < next - 5 )
                        {
                            shouldBreak = true;
                        }
                    }

                    if(shouldBreak)
                    {
                        canvas.SetStrokeColor(ColorConstants.YELLOW);
                        canvas.Rectangle(blockSet.GetX(), blockSet.GetH(), blockSet.GetWidth(), blockSet.GetHeight());
                        canvas.Stroke();

                        // reset
                        blockSet = new BlockSet();
                    }

                    blockSet.Add(b);

                    last = b;
                }));
                
                parser.ProcessPageContent(page);

                if (blockSet != null)
                {
                    canvas.SetStrokeColor(ColorConstants.YELLOW);
                    canvas.Rectangle(blockSet.GetX(), blockSet.GetH(), blockSet.GetWidth(), blockSet.GetHeight());
                    canvas.Stroke();
                }
            }
        }

        public void ExampleMarker(Block b, PdfCanvas canvas)
        {
            // letra maiuscula: if(b.Text.Length > 0 &&  (b.Text.ToUpper()[0] == b.Text[0]) )
            canvas.SetStrokeColor(ColorConstants.YELLOW);
            canvas.Rectangle(b.X, b.B, b.Width, b.Height - b.Lower);
            canvas.Stroke();
        }

        public void ProcessMarker2(string srcpath, string dstpath)
        {
            using (var pdf = new PdfDocument(new PdfReader(srcpath), new PdfWriter(dstpath)))
            {
                var page = pdf.GetPage(1);
                var canvas = new PdfCanvas(page);

                var parser = new PdfCanvasProcessor(new UserListener(b => {
                    canvas.SetStrokeColor(ColorConstants.YELLOW);
                    canvas.Rectangle(b.X, b.B, b.Width, b.Height - b.Lower);
                    canvas.Stroke();
                }));

                parser.ProcessPageContent(page);
            }
        }

    }
}
