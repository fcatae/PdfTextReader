using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader
{
    class PdfWriteText
    {
        public static void Test()
        {
            //var pdfOutput = new PdfDocument(new PdfReader("bin/p40"), new PdfWriter("bin/write"));

            PdfDocument pdf = new PdfDocument(new PdfWriter("bin/write.pdf"));
            //PdfPage page = pdf.AddNewPage();
            //PdfCanvas pdfCanvas = new PdfCanvas(page);
            //pdfCanvas.Rectangle(rectangle);
            //pdfCanvas.Stroke();
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.COURIER);
            PdfFont bold = PdfFontFactory.CreateFont(StandardFonts.COURIER_BOLD);

            Text title = new Text("title abc de e ff ");
            Text author = new Text("Robert Louis Stevenson").SetFont(font);

            //Paragraph p = new Paragraph().Add(title).Add(" by ").Add(author);

            //Rectangle rectangle = new Rectangle(36, 650, 100, 100);


            //Rectangle rect2 = new Rectangle(136, 450, 300, 300);
            //var page2 = pdf.AddNewPage();
            //var pdfcanvas2 = new PdfCanvas(page2);
            //var canvas2 = new Canvas(pdfcanvas2, pdf, rect2);
            //pdfcanvas2.Rectangle(rect2);
            //pdfcanvas2.Stroke();
            //canvas2.Add(new Paragraph("abc"));
            //canvas2.Close();
            var page3 = pdf.AddNewPage();
            var page4 = pdf.AddNewPage();
            var page5 = pdf.AddNewPage();

            //Print(page5, 5);
            //Print(page4, 4);
            //Print(page3, 3);

            //Print2(page5, 5);
            //Print2(page4, 4);
            //Print2(page3, 3);

            Print3(page5, 5);
            Print3(page4, 4);
            Print3(page3, 3);

            //// canvas 1
            //Canvas canvas = new Canvas(pdfCanvas, pdf, rectangle);
            //canvas.Add(p);
            //canvas.Close();
            
            pdf.Close();
        }

        static void Print(PdfPage page, int i)
        {
            PdfCanvas canvas = new PdfCanvas(page);

            Rectangle rectangle = new Rectangle(30, 530, 100, 100);
            Canvas box = new Canvas(canvas, page.GetDocument(), rectangle, true);
            
            canvas.SetFillColor(iText.Kernel.Colors.ColorConstants.GREEN);
            canvas.Circle(30, 530, 20);
            canvas.Fill();

            PdfAction action = PdfAction.CreateURI("www.bing.com/?q=" + i.ToString());
            Link link = new Link("blablabla", action);

            var t = new Text(i.ToString());

            var p = new Paragraph(t);
            p = p.SetVerticalAlignment(iText.Layout.Properties.VerticalAlignment.BOTTOM);
            box.Add(p);
            //box.Add(new Paragraph(link));
            box.Close();

            
        }
        static void Print2(PdfPage page, int i)
        {
            PdfCanvas canvas = new PdfCanvas(page);

            Rectangle rectangle = new Rectangle(230, 530, 100, 100);
            Canvas box = new Canvas(canvas, page.GetDocument(), rectangle);

            canvas.SetFillColor(iText.Kernel.Colors.ColorConstants.GREEN);
            canvas.SetStrokeColor(iText.Kernel.Colors.ColorConstants.BLUE);
            canvas.Circle(230, 530, 20);
            canvas.RoundRectangle(230, 530, 100, 100, 20);
            canvas.Stroke();
                        
            var t = new Text(i.ToString());
            var p = new Paragraph(t);
            var p2 = new Paragraph(t);
            p.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
            p.SetVerticalAlignment(iText.Layout.Properties.VerticalAlignment.BOTTOM);
            box.Add(p);
            box.Close();
        }

        static void Print3(PdfPage page, int i)
        {
            PdfCanvas canvas = new PdfCanvas(page);

            //Rectangle rectangle = new Rectangle(30, 530, 100, 100);
            //Canvas box = new Canvas(canvas, page.GetDocument(), rectangle, true);

            
            canvas.BeginText();
            //canvas.MoveTo(30, 100);
            canvas.MoveText(30, 100);
            canvas.SetColor(iText.Kernel.Colors.ColorConstants.RED, true);
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER_BOLD), 10);
            canvas.ShowText("YEAH");
            canvas.EndText();


        }
    }
}
