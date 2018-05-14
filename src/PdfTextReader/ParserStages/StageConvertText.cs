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

            var result = pipeline.Input($"{_context.InputFilePrefix}.pdf")
                    .AllPages<CreateTextLines>(GetLines)
                    .ConvertText<CreateTextLineIndex, TextLine>(true)
                    .ConvertText<PreCreateStructures, TextLine2>()
                    .ConvertText<CreateStructures3, TextStructure>()
                    .ConvertText<PreCreateTextSegments, TextStructureAgg>()
                    .ConvertText<AggregateStructures, TextStructure>(true)
                        .ShowPdf<ShowStructureCentral>($"{_context.OutputFilePrefix}-show-central.pdf")
                        .Log<AnalyzeStructures>($"{_context.OutputFilePrefix}-analyze-structures.txt")
                        .Log<AnalyzeStructuresCentral>($"{_context.OutputFilePrefix}-analyze-structures-central.txt");

            _context.SetPipelineText<TextStructure>(result);
        }

        void GetLines(PipelineInputPdf.PipelineInputPdfPage page)
        {
            page.FromCache<FinalBlockResultData>();
        }
    }
}
