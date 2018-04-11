using PdfTextReader.Base;
using PdfTextReader.Execution;
using PdfTextReader.ExecutionStats;
using PdfTextReader.Parser;
using PdfTextReader.PDFCore;
using PdfTextReader.PDFText;
using PdfTextReader.TextStructures;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace PdfTextReader
{
    class Examples
    {
        public static PipelineText<TextLine> GetTextLines(string inputname, string outputname, out Execution.Pipeline pipeline)
        {
            pipeline = new Execution.Pipeline();

            return GetTextLines(pipeline, inputname, outputname);
        }

        public static PipelineText<TextLine> GetTextLines(Execution.Pipeline pipeline, string inputname, string outputname)
        {
            var result =
            pipeline.Input(inputname)
                    .Output(outputname)
                    .AllPages<CreateTextLines>(page =>
                              page.ParsePdf<PreProcessTables>()
                                  .ParseBlock<IdentifyTables>()             // 1
                              .ParsePdf<PreProcessImages>()
                                  .ParseBlock<BasicFirstPageStats>()        // 2
                                  .ParseBlock<RemoveOverlapedImages>()      // 3
                              .ParsePdf<ProcessPdfText>()                   // 4
                                  .ParseBlock<RemoveSmallFonts>()           // 5
                                  .ParseBlock<MergeTableText>()             // 6
                                  .ParseBlock<HighlightTextTable>()         // 7
                                  .ParseBlock<RemoveTableText>()            // 8
                                  .ParseBlock<ReplaceCharacters>()          // 9
                                  .ParseBlock<GroupLines>()                 // 10
                                  .ParseBlock<RemoveTableDotChar>()         // 11
                                      .Show(Color.Yellow)
                                      .Validate<RemoveHeaderImage>().ShowErrors(p => p.Show(Color.Purple))
                                  .ParseBlock<RemoveHeaderImage>()              // 12
                                  .ParseBlock<FindInitialBlocksetWithRewind>()  // 13
                                      .Show(Color.Gray)
                                  .ParseBlock<BreakColumnsLight>()          // 14
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

                                  .PrintWarnings()
                    );

            return result;
        }
    }
}
