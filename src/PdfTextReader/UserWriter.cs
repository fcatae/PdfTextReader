using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Layout;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

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

        public void ProcessBlock(string srcpath, string dstpath)
        {
            using (var pdf = new PdfDocument(new PdfReader(srcpath), new PdfWriter(dstpath)))
            {
                var page = pdf.GetPage(1);
                var canvas = new PdfCanvas(page);
                var blockList = new List<BlockSet>();
                var blockSet = new BlockSet();
                Block last = null;

                var parser = new PdfCanvasProcessor(new UserListener( b => {

                    bool shouldBreak = false;
                    
                    if( last != null )
                    {
                        // expect: previous >~ next
                        float previous = last.H;
                        float next = b.H;

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
                        blockList.Add(blockSet);

                        // reset
                        blockSet = new BlockSet();
                    }

                    blockSet.Add(b);

                    last = b;
                }));
                
                parser.ProcessPageContent(page);

                blockList.Add(blockSet);

                var footer = FindFooter(blockList);
                var header = FindHeader(blockList);

                // post-processing
                DrawRectangle(canvas, blockList, ColorConstants.YELLOW);
                DrawRectangle(canvas, footer, ColorConstants.BLUE);
                DrawRectangle(canvas, header, ColorConstants.BLUE);

            }
        }

        void DrawRectangle(PdfCanvas canvas, IEnumerable<BlockSet> blockList, Color color)
        {
            foreach (var bset in blockList)
            {
                canvas.SetStrokeColor(color);
                canvas.Rectangle(bset.GetX(), bset.GetH(), bset.GetWidth(), bset.GetHeight());
                canvas.Stroke();
            }
        }

        IEnumerable<BlockSet> FindHeader(List<BlockSet> blockList)
        {
            float err = 1f;
            float maxH = blockList.Max(b => b.GetH()) - err;

            var blocksAtHeader = blockList.Where(b => b.GetH() >= maxH);

            return blocksAtHeader.ToArray();
        }

        IEnumerable<BlockSet> FindFooter(List<BlockSet> blockList)
        {
            float err = 1f;
            float minH = blockList.Min(b => b.GetH()) + err;

            var blocksAtFooter = blockList.Where(b => b.GetH() <= minH);

            return blocksAtFooter.ToArray();
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
