using PdfTextReader.Execution;
using PdfTextReader.PDFText;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PdfTextReader
{
    public class ExamplesAzure
    {
        public static void FollowText(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"{basename}.pdf")
                    .Output($"{basename}-follow-text-output.pdf")
                    .AllPages(page => page
                              .ParsePdf<ProcessPdfText>()
                              .ShowLine(Color.Orange)
                    );

            pipeline.Done();
        }
    }
}
