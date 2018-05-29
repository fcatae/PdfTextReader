using PdfTextReader.Execution;
using PdfTextReader.Parser;
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

            var page = pipeline.Input($"{_context.InputFilePrefix}.pdf")
                    .Page(1)
                    .ParsePdf<ProcessPdfText>()
                    .ParseBlock<ExtractDouHeaderInfo>();

            var extract = page.CreateInstance<ExtractDouHeaderInfo>();
            var infoStats = extract.InfoStats;
            string content = infoStats.ToString();

            var filename = _context.CreateGlobalInstance<InjectFilename>();
            filename.Filename = _context.Basename;
            filename.InfoStats = infoStats;

            _context.WriteFile("header", $"{_context.OutputFilePrefix}-header.txt", content);
        }
    }
}
