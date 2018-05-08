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
    class StageConvertTree
    {
        private readonly string _input;
        private readonly string _output;
        private readonly StageContext _context;

        public StageConvertTree(StageContext context)
        {
            this._input = context.InputFolder;
            this._output = context.OutputFolder;
            this._context = context;
        }

        public void Process()
        {
            var pipelineText = _context.GetPipelineText<TextSegment>();

            var resultPipeline = pipelineText
                            .ConvertText<CreateTreeSegments, TextSegment>()
                            .ConvertText<MergeTreeSegments, TextSegment>(true)
                                .Log<AnalyzeSegmentTitles>($"{_context.OutputFilePrefix}-analyze-segment-titles.txt")
                                .Log<AnalyzeTreeStructure>($"{_context.OutputFilePrefix}-tree.txt");

            _context.SetPipelineText<TextSegment>(resultPipeline);

            _context.AddOutput("tree", $"{_context.OutputFilePrefix}-tree.txt");
        }

    }
}
