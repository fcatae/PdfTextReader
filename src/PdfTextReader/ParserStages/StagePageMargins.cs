using PdfTextReader.Execution;
using PdfTextReader.PDFCore;
using PdfTextReader.PDFText;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PdfTextReader.ParserStages
{
    class StagePageMargins
    {
        private readonly StageContext _context;

        public StagePageMargins(StageContext context)
        {
            this._context = context;
        }

        public void Process()
        {
            Pipeline pipeline = _context.GetPipeline();
            
            pipeline.Input($"{_context.InputFilePrefix}.pdf")
                    .StageProcess(FindMargins);

            pipeline.Input($"{_context.InputFilePrefix}.pdf")
                    .Output($"{_context.OutputFilePrefix}-stage1-margins.pdf")
                    .StageProcess(ShowColors);
            
            _context.AddOutput("stage1-margins", $"{_context.OutputFilePrefix}-stage1-margins.pdf");
        }

        void FindMargins(PipelineInputPdf.PipelineInputPdfPage page)
        {
            page
                .FromCache<IdentifyTablesData>()
                .FromCache<ProcessImageData>()
                .FromCache<ProcessPdfTextData>()
                  .ParseBlock<FindDouHeaderFooter>()
                        .StoreCache<HeaderFooterData>();
        }

        void ShowColors(PipelineInputPdf.PipelineInputPdfPage page)
        {
            page
                .FromCache<HeaderFooterData>()
                .FromCache<ProcessPdfTextData>()
                  .Validate<ShowTextHeaderFooter>().ShowErrors(p => p.Show(Color.PaleVioletRed))
                  .ParseBlock<ShowTextHeaderFooter>()
                    .Show(Color.Yellow);
        }
    }
}
