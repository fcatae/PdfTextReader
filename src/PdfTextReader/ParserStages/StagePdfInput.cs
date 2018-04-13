using PdfTextReader.Execution;
using PdfTextReader.PDFCore;
using PdfTextReader.PDFText;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PdfTextReader.ParserStages
{
    class StagePdfInput
    {
        const string INPUT = "input";
        const string OUTPUT = "output";

        //public StagePdfInput(StageContext context)
        //{
        //    // define the file system
        //}

        public void Process(string basename)
        {
            using (Pipeline pipeline = new Pipeline())
            {
                pipeline.Input($"{INPUT}/{basename}.pdf")
                        .StageProcess(InitialCache);

                pipeline.Input($"{INPUT}/{basename}.pdf")
                        .Output($"{OUTPUT}/{basename}/stage0-input.pdf")
                        .StageProcess(ShowColors);
            }
        }

        void InitialCache(PipelineInputPdf.PipelineInputPdfPage page)
        {
            page.ParsePdf<PreProcessTables>()
                    .ParseBlock<IdentifyTables>().StoreCache<IdentifyTables>()
                .ParsePdf<PreProcessImages>().StoreCache<PreProcessImages>()
                .ParsePdf<ProcessPdfText>().StoreCache<ProcessPdfText>();
        }

        void ShowColors(PipelineInputPdf.PipelineInputPdfPage page)
        {
            page.FromCache<IdentifyTables>()
                    .Show(Color.Yellow)
                .FromCache<PreProcessImages>()
                    .Show(Color.Orange)
                .FromCache<ProcessPdfText>()
                    .Show(Color.Blue);
        }
    }
}
