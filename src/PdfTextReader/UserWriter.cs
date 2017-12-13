using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
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
    }
}
