using PdfTextReader.Base;
using PdfTextReader.Parser;
using PdfTextReader.TextStructures;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.ParserStages
{
    class StageConvertStructText
    {
        private readonly string _input;
        private readonly string _output;
        private readonly StageContext _context;

        public StageConvertStructText(StageContext context)
        {
            this._input = context.InputFolder;
            this._output = context.OutputFolder;
            this._context = context;
        }

        public void Process()
        {
            var pipelineText = _context.GetPipelineText<TextSegment>();

            var resultPipeline = pipelineText
                            .ConvertText<CreateStructText, TextSegment>(true)
                            .Log<AnalyzeSegmentTextVersion>($"{_context.OutputFilePrefix}-text-version.txt");

            _context.SetPipelineText<TextSegment>(resultPipeline);
        }
    }
}
