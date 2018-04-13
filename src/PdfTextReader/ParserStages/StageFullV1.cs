using PdfTextReader.Execution;
using PdfTextReader.PDFCore;
using PdfTextReader.PDFText;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PdfTextReader.ParserStages
{
    class StageFullV1
    {
        private readonly string _input;
        private readonly string _output;
        private readonly StageContext _context;

        public StageFullV1(StageContext context)
        {
            this._input = context.InputFolder;
            this._output = context.OutputFolder;
            this._context = context;
        }

        public void Process()
        {
            string basename = _context.Basename;
            Pipeline pipeline = _context.GetPipeline();

            pipeline.Input($"{_context.InputFilePrefix}.pdf")
                    .Output($"{_context.OutputFilePrefix}-output.pdf")
                    .StageProcess(ProcessFull);
        }

        void ProcessFull(PipelineInputPdf.PipelineInputPdfPage page)
        {
            page.ParsePdf<PreProcessTables>()
                    .ParseBlock<IdentifyTables>()             // 1
                .ParsePdf<PreProcessImages>()
                    .ParseBlock<BasicFirstPageStats>()        // 2
                                                              //.Validate<RemoveOverlapedImages>().ShowErrors(p => p.Show(Color.Blue))
                    .ParseBlock<RemoveOverlapedImages>()      // 3
                .ParsePdf<ProcessPdfText>()                   // 4
                                                              //.Validate<RemoveSmallFonts>().ShowErrors(p => p.ShowText(Color.Green))
                    .ParseBlock<RemoveSmallFonts>()           // 5
                                                              //.Validate<MergeTableText>().ShowErrors(p => p.Show(Color.Blue))
                    .ParseBlock<MergeTableText>()             // 6
                                                              //.Validate<HighlightTextTable>().ShowErrors(p => p.Show(Color.Green))
                    .ParseBlock<HighlightTextTable>()         // 7
                    .ParseBlock<RemoveTableText>()            // 8
                    .ParseBlock<ReplaceCharacters>()          // 9
                    .ParseBlock<GroupLines>()                 // 10
                    .ParseBlock<RemoveTableDotChar>()         // 11
                        .Show(Color.Yellow)
                        .Validate<RemoveHeaderImage>().ShowErrors(p => p.Show(Color.Purple))
                    .ParseBlock<RemoveHeaderImage>()          // 12
                    .ParseBlock<FindInitialBlocksetWithRewind>()  // 13
                        .Show(Color.Gray)
                    .ParseBlock<BreakColumnsLight>()          // 14
                                                              //.ParseBlock<BreakColumns>()
                    .ParseBlock<AddTableSpace>()              // 15
                    .ParseBlock<RemoveTableOverImage>()       // 16
                    .ParseBlock<RemoveImageTexts>()           // 17
                    .ParseBlock<AddImageSpace>()              // 18
                        .Validate<RemoveFooter>().ShowErrors(p => p.Show(Color.Purple))
                    .ParseBlock<RemoveFooter>()               // 19
                    .ParseBlock<AddTableHorizontalLines>()    // 20
                    .ParseBlock<RemoveBackgroundNonText>()    // 21
                        .ParseBlock<BreakColumnsRewrite>()    // 22

                    .ParseBlock<BreakInlineElements>()        // 23
                    .ParseBlock<ResizeBlocksets>()            // 24
                    .ParseBlock<ResizeBlocksetMagins>()       // 25
                    .ParseBlock<OrderBlocksets>()           // 26

                    .ParseBlock<OrganizePageLayout>()         // 27
                    .ParseBlock<MergeSequentialLayout>()      // 28
                    .ParseBlock<ResizeSequentialLayout>()     // 29
                        .Show(Color.Orange)
                        .ShowLine(Color.Black)

                    .ParseBlock<CheckOverlap>()               // 30

                        .Validate<CheckOverlap>().ShowErrors(p => p.Show(Color.Red))
                        .Validate<ValidatePositiveCoordinates>().ShowErrors(p => p.Show(Color.Red))
                    .PrintWarnings();
        }
    }
}
