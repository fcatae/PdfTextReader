using PdfTextReader.Execution;
using PdfTextReader.PDFCore;
using PdfTextReader.PDFText;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PdfTextReader.ParserStages
{
    class StageBlocksets
    {
        private readonly StageContext _context;

        public StageBlocksets(StageContext context)
        {
            this._context = context;
        }

        public void Process()
        {
            Pipeline pipeline = _context.GetPipeline();

            pipeline.Input($"{_context.InputFilePrefix}.pdf")
                    .Output($"{_context.OutputFilePrefix}-stage2-blocksets.pdf")
                    .StageProcess(FindBlocksets);
        }

        void FindBlocksets(PipelineInputPdf.PipelineInputPdfPage page)
        {
            page
                .FromCache<IdentifyTablesData>()
                    .ParseBlock<SetIdentifyTablesCompatibility>()
                .FromCache<ProcessImageData>()
                    .ParseBlock<SetProcessImageCompatibility>()
                    .ParseBlock<BasicFirstPageStats>()          // 2
                    .ParseBlock<RemoveOverlapedImages2>()       // 3
                
                .FromCache<HeaderFooterData>()

                .FromCache<ProcessPdfTextData>()                // 4
                    .ParseBlock<FilterHeaderFooter>()
                    .ParseBlock<RemoveSmallFonts>()           // 5
                    .ParseBlock<MergeTableText>()             // 6

                    //.ParseBlock<HighlightTextTable>()         // 7

                    .ParseBlock<RemoveTableText>()            // 8

                    //.ParseBlock<ReplaceCharacters>()          // 9

                    .ParseBlock<GroupLines>()                 // 10

                    //.ParseBlock<RemoveTableDotChar>()         // 11
                        .Show(Color.Yellow);

        //    .Validate<RemoveHeaderImage>().ShowErrors(p => p.Show(Color.Purple))
        //.ParseBlock<RemoveHeaderImage>()          // 12

        //            .ParseBlock<FindInitialBlocksetWithRewind>()  // 13
        //                .Show(Color.Gray)
        //            .ParseBlock<BreakColumnsLight>()          // 14
        //            .ParseBlock<AddTableSpace>()              // 15
        //            .ParseBlock<RemoveTableOverImage>()       // 16
        //            .ParseBlock<RemoveImageTexts>()           // 17
        //            .ParseBlock<AddImageSpace>()              // 18

        //                .Validate<RemoveFooter>().ShowErrors(p => p.Show(Color.Purple))
        //            .ParseBlock<RemoveFooter>()               // 19

        //            .ParseBlock<AddTableHorizontalLines>()    // 20
        //            .ParseBlock<RemoveBackgroundNonText>()    // 21
        //                .ParseBlock<BreakColumnsRewrite>()    // 22

        //            .ParseBlock<BreakInlineElements>()        // 23
        //            .ParseBlock<ResizeBlocksets>()            // 24
        //            .ParseBlock<ResizeBlocksetMagins>()       // 25
        //            .ParseBlock<OrderBlocksets>()           // 26

        //            .ParseBlock<OrganizePageLayout>()         // 27
        //            .ParseBlock<MergeSequentialLayout>()      // 28
        //            .ParseBlock<ResizeSequentialLayout>()     // 29
        //                .Show(Color.Orange)
        //                .ShowLine(Color.Black)

        //            .ParseBlock<CheckOverlap>()               // 30
        //                .Validate<CheckOverlap>().ShowErrors(p => p.Show(Color.Red))
        //                .Validate<ValidatePositiveCoordinates>().ShowErrors(p => p.Show(Color.Red))
        //            .PrintWarnings();
        }
    }
}
