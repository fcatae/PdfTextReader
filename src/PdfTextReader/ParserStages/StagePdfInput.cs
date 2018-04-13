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
        private readonly string _input;
        private readonly string _output;
        private readonly StageContext _context;

        public StagePdfInput(StageContext context)
        {
            this._input = context.InputFolder;
            this._output = context.OutputFolder;
            this._context = context;
        }

        public void Process()
        {
            string basename = _context.Basename;
            Pipeline pipeline = _context.GetPipeline();

            pipeline.Input($"{_context.InputFilePrefix}.pdf")
                    .StageProcess(InitialCache);

            pipeline.Input($"{_context.InputFilePrefix}.pdf")
                    .Output($"{_context.OutputFilePrefix}-stage0-input.pdf")
                    .StageProcess(ShowColors);
        }

        void InitialCache(PipelineInputPdf.PipelineInputPdfPage page)
        {
            page.ParsePdf<PreProcessTables>()
                    .ParseBlock<IdentifyTables>().StoreCache<IdentifyTables>()
                .ParsePdf<PreProcessImages>()
                    .ParseBlock<ProcessImageData>().StoreCache<ProcessImageData>()
                .ParsePdf<ProcessPdfText>().StoreCache<ProcessPdfText>();
        }

        void ShowColors(PipelineInputPdf.PipelineInputPdfPage page)
        {
            page
                .FromCache<ProcessPdfText>()
                    .Show(Color.Yellow)
                .FromCache<ProcessImageData>()
                    .Show(Color.Blue)
                .FromCache<IdentifyTables>()
                    .Show(Color.Red);
        }
    }
}
