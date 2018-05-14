using PdfTextReader.Execution;
using PdfTextReader.PDFCore;
using PdfTextReader.PDFText;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PdfTextReader.ParserStages
{
    class StageDbgFlow
    {
        private readonly string _input;
        private readonly string _output;
        private readonly StageContext _context;

        public StageDbgFlow(StageContext context)
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
                    .Output($"{_context.OutputFilePrefix}-dbg0-flow.pdf")
                    .StageProcess(Flow);
        }

        void Flow(PipelineInputPdf.PipelineInputPdfPage page)
        {
            page.ParsePdf<PreProcessTables>()
                    .Show(Color.Blue)
                .ParsePdf<PreProcessImages>()                    
                    .Show(Color.Orange)
                .ParsePdf<ProcessPdfText>()
                    .Show(Color.Yellow)
                    .ShowLine(Color.Black)                    ;
        }
    }
}
