using PdfTextReader.Execution;
using PdfTextReader.PDFCore;
using PdfTextReader.PDFText;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PdfTextReader.ParserStages
{
    class StageExtractHeaderDOU
    {
        private readonly string _input;
        private readonly string _output;
        private readonly StageContext _context;

        public StageExtractHeaderDOU(StageContext context)
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
                    .StageProcess(FindHeaderInformation);
        }

        void FindHeaderInformation(PipelineInputPdf.PipelineInputPdfPage page)
        {
            page
                .Global<PageInfoStats>()
                .ParsePdf<ProcessPdfText>()
                .ParseBlock<ExtractDouHeaderInfo>();
        }
    }
}
