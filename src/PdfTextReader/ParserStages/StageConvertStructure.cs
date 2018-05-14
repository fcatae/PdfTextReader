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
    class StageConvertStructure
    {
        private readonly string _input;
        private readonly string _output;
        private readonly StageContext _context;

        public StageConvertStructure(StageContext context)
        {
            this._input = context.InputFolder;
            this._output = context.OutputFolder;
            this._context = context;
        }

        public void Process()
        {
            var pipelineText = _context.GetPipelineText<TextLine>();

            var resultPipeline = pipelineText
                            .ConvertText<PreCreateStructures, TextLine2>()
                            .ConvertText<CreateStructures3, TextStructure>()
                            .ConvertText<PreCreateTextSegments, TextStructureAgg>()
                            .ConvertText<AggregateStructures, TextStructure>(true)
                                .ShowPdf<ShowStructureCentral>($"{_context.OutputFilePrefix}-show-central.pdf")
                                .Log<AnalyzeStructures>($"{_context.OutputFilePrefix}-analyze-structures.txt")
                                .Log<AnalyzeStructuresCentral>($"{_context.OutputFilePrefix}-analyze-structures-central.txt")

                            .ConvertText<CreateTextSegmentsWithConfigData, TextSegment>();

                            //.ConvertText<CreateTextSegments, TextSegment>()
                            //.ConvertText<FilterTextSegments, TextSegment>()
                            //.ConvertText<AfterFilterTextSegments, TextSegment>();

            _context.SetPipelineText<TextSegment>(resultPipeline);
        }

    }
}
