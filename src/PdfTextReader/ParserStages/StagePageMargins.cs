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
            page.FromCache<IdentifyTables>()
                              .FromCache<ProcessImageData>()
                                .ParseBlock<ProcessImageData>()
                                  .ParseBlock<RemoveOverlapedImages2>()
                              .FromCache<ProcessPdfText>()

                                  .ParseBlock<RemoveHeaderImage>()
                                  .ParseBlock<FindInitialBlockset>() 
                                  .ParseBlock<AddTableSpace>()       
                                  .ParseBlock<RemoveFooter>()
                                                                        .Show(Color.Gray)

                                  ;
        }
    }
}
