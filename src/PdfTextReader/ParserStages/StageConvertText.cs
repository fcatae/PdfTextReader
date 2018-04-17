using PdfTextReader.Base;
using PdfTextReader.Execution;
using PdfTextReader.Parser;
using PdfTextReader.PDFCore;
using PdfTextReader.TextStructures;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.ParserStages
{
    class StageConvertText
    {
        private readonly string _input;
        private readonly string _output;
        private readonly StageContext _context;

        public StageConvertText(StageContext context)
        {
            this._input = context.InputFolder;
            this._output = context.OutputFolder;
            this._context = context;
        }

        public void Process()
        {
            Pipeline pipeline = _context.GetPipeline();

            var pipelineText = pipeline.Input($"{_context.InputFilePrefix}.pdf")
                    .AllPages<CreateTextLines>(GetLines)
                    .ConvertText<CreateTextLineIndex, TextLine>(true);

            _context.SetPipelineText<TextLine>(pipelineText);
        }

        void GetLines(PipelineInputPdf.PipelineInputPdfPage page)
        {
            page.FromCache<FinalBlockResultData>();
        }
    }
}
