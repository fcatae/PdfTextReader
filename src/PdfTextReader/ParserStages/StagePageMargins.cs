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
                    .StageProcess(FindMarings);
        }

        void FindMarings(PipelineInputPdf.PipelineInputPdfPage page)
        {
            page.FromCache<IdentifyTablesData>()
                .ParseBlock<SetIdentifyTablesCompatibility>()

              .FromCache<ProcessImageData>()
                .ParseBlock<SetProcessImageCompatibility>()
                .ParseBlock<RemoveOverlapedImages2>()
              .FromCache<ProcessPdfTextData>()

                  .Validate<FindDouHeaderFooter>().ShowErrors(p => p.Show(Color.Purple))
                  .ParseBlock<FindDouHeaderFooter>()
                  .Show(Color.Yellow);
                  
                  //.Validate<RemoveHeaderImage>().ShowErrors(p => p.Show(Color.Purple))
                  //.ParseBlock<RemoveHeaderImage>()
                  //.ParseBlock<FindInitialBlockset>()
                  //.ParseBlock<AddTableSpace>()
                  //.Validate<RemoveFooter>().ShowErrors(p => p.Show(Color.Purple))
                  //.ParseBlock<RemoveFooter>();
        }
    }
}
