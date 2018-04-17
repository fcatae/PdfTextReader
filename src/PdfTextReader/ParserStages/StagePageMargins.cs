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
                    .Output($"{_context.OutputFilePrefix}-stage1-margins.pdf")
                    .StageProcess(FindMargins);
        }

        void FindMargins(PipelineInputPdf.PipelineInputPdfPage page)
        {
            page
                .FromCache<IdentifyTablesData>()
                .FromCache<ProcessImageData>()
                .FromCache<ProcessPdfTextData>()

                  .ParseBlock<FindDouHeaderFooter>()

                  .Validate<FilterHeaderFooter>().ShowErrors(p => p.Show(Color.Purple))
                  .ParseBlock<FilterHeaderFooter>()
                        .StoreCache<HeaderFooterData>()
                  .Show(Color.Yellow);
        }
    }
}
