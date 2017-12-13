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
                var tmp = new BlockSet();
                int count = 0;

                var parser = new PdfCanvasProcessor(new UserListener( b => {

                    count++;

                    tmp.Add(b);

                    if(count > 20)
                    {
                        var ttmp = new
                        {
                            a = tmp.GetX(),
                            b = tmp.GetH(),
                            c = tmp.GetWidth(),
                            d = tmp.GetHeight()
                        };

                        canvas.SetStrokeColor(ColorConstants.YELLOW);
                        canvas.Rectangle(tmp.GetX(), tmp.GetH(), tmp.GetWidth(), tmp.GetHeight());
                        canvas.Stroke();

                        // reset
                        tmp = new BlockSet();
                        count = 0;
                    }

                }));

                parser.ProcessPageContent(page);
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
