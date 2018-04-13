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
                    .StageProcess(ShowColors);
        }

        void ShowColors(PipelineInputPdf.PipelineInputPdfPage page)
        {
            page.FromCache<IdentifyTablesData>()
                // does not work yet    
                //.ParseBlock<SetIdentifyTablesCompatibility>()

              .FromCache<ProcessImageData>()
                .ParseBlock<ProcessImageData>()
                .ParseBlock<SetProcessImageCompatibility>()
                .ParseBlock<RemoveOverlapedImages2>()
              .FromCache<ProcessPdfText>()
                  .Validate<RemoveHeaderImage>().ShowErrors(p => p.Show(Color.Green))
                  .ParseBlock<RemoveHeaderImage>()
                  .ParseBlock<FindInitialBlockset>()
                  //.ParseBlock<AddTableSpace>()       
                  .ParseBlock<RemoveFooter>()
                  
                .Show(Color.Gray);
        }
    }
}
