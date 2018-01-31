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
using System.Text;

namespace PdfTextReader
{
    public class ExamplesAzure
    {
        public static void FollowText(IVirtualFS virtualFS, string basename)
        {
            VirtualFS.ConfigureFileSystem(virtualFS);

            PdfReaderException.ContinueOnException();

            var pipeline = new Execution.Pipeline();

            pipeline.Input($"{basename}.pdf")
                    .Output($"{basename}-follow-text-output.pdf")
                    .AllPages(page => page
                              .ParsePdf<ProcessPdfText>()
                              .ShowLine(Color.Orange)
                    );

            pipeline.Done();
        }

        public static void RunParserPDF(IVirtualFS virtualFS, string basename, string inputfolder, string outputfolder)
        {
            VirtualFS.ConfigureFileSystem(virtualFS);

            PdfReaderException.ContinueOnException();

            Pipeline pipeline = new Pipeline();

            var artigos = GetTextLines(pipeline, basename, inputfolder, outputfolder)
                                .Log<AnalyzeLines>($"{outputfolder}/{basename}/lines.txt")
                            .ConvertText<CreateTextLineIndex, TextLine>()
                            .ConvertText<PreCreateStructures, TextLine2>()
                            .ConvertText<CreateStructures2, TextStructure>()
                            .ConvertText<PreCreateTextSegments, TextStructureAgg>()
                            .ConvertText<AggregateStructures, TextStructure>()
                                .ShowPdf<ShowStructureCentral>($"{outputfolder}/{basename}/show-central.pdf")
                                .Log<AnalyzeStructures>($"{outputfolder}/{basename}/struct.txt")
                                .Log<AnalyzeStructuresCentral>($"{outputfolder}/{basename}/central.txt")
                            .ConvertText<CreateTextSegments, TextSegment>()
                            .ConvertText<CreateTreeSegments, TextSegment>()
                                .Log<AnalyzeSegmentTitles>($"{outputfolder}/{basename}/segment-titles-tree.txt")
                                .Log<AnalyzeTreeStructure>(Console.Out)
                            .ToList();
            
            //pipeline.ExtractOutput<ShowParserWarnings>($"{outputfolder}/{basename}/parser-errors.pdf");
        }

        static PipelineText<TextLine> GetTextLines(Pipeline pipeline, string basename, string inputfolder, string outputfolder)
        {
            var result =
            pipeline.Input($"{inputfolder}/{basename}.pdf")
                    .Output($"{outputfolder}/{basename}/parser-output.pdf")
                    .AllPages<CreateTextLines>(page =>
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
                                  .PrintWarnings()
                    );

            return result;
        }
    }
}
