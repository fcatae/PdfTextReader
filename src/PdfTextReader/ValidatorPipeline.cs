using System;
using System.Collections.Generic;
using System.Text;
using PdfTextReader.Base;
using PdfTextReader.Parser;
using PdfTextReader.ExecutionStats;
using PdfTextReader.TextStructures;
using PdfTextReader.Execution;
using PdfTextReader.PDFText;
using System.Drawing;
using PdfTextReader.PDFCore;
using PdfTextReader.PDFValidation;

namespace PdfTextReader
{
    public class ValidatorPipeline
    {
        public static int Process(string basename, string inputfolder, string outputfolder)
        {
            //PdfReaderException.DisableWarnings();
            //PdfReaderException.ContinueOnException();
            
            var pipeline = new Execution.Pipeline();

            var result =
            pipeline.Input($"{inputfolder}/{basename}")
                    //.Output($"{outputfolder}/{basename}-output.pdf")
                    .AllPagesExcept<CreateTextLines>(new int[] { }, page =>
                              page.ParsePdf<ProcessPdfValidation>()
                                  //.Show(Color.White)
                                  .ParseBlock<IdentifyValidationMarks>()
                                  .PdfCheck<CheckNoBlockSetOverlap>(Color.Orange)   
                                  //.Show(Color.Blue)
                    ).ToList();

            //pipeline.SaveOk($"{outputfolder}/{basename}-ok.pdf");
            int errors = pipeline.SaveErrors($"{outputfolder}/errors/{basename}-errors.pdf");

            pipeline.Done();

            return errors;
        }
        public static void ProcessPage1(string basename, string inputfolder, string outputfolder)
        {
            //PdfReaderException.DisableWarnings();
            //PdfReaderException.ContinueOnException();

            var pipeline = new Execution.Pipeline();

            var result =
            pipeline.Input($"{inputfolder}/{basename}")
                    .Output($"{outputfolder}/{basename}-invalid.pdf")
                    .Page(1)
                                .ParsePdf<ProcessPdfValidation>()
                                  .Show(Color.White)
                                  .ParseBlock<IdentifyValidationMarks>()
                                  .ParseBlock<CheckNoBlockSetOverlap>()
                                  .Show(Color.Blue);
                    
            pipeline.Done();
       }
    }
}
