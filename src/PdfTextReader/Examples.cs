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
        public static void FollowText(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/{basename}-follow-text-output.pdf")
                    .Page(1)
                    .ParsePdf<ProcessPdfText>()
                    .ShowLine(Color.Orange);

            pipeline.Done();
        }
        public static void ShowTables(string basename)
        {            
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/{basename}-tables.pdf")
                    .Page(1)
                    .ParsePdf<PreProcessTables>()
                        .ParseBlock<IdentifyTables>()
                        .Show(Color.Green)
                        .Validate<CheckOverlap>().ShowErrors( b => b.Show(Color.Red))
                    ;

            pipeline.Done();
        }

        public static void ShowHeaderFooter(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/{basename}-header-footer-output.pdf")
                    .AllPages(page =>
                    {
                        page.ParsePdf<PreProcessTables>()
                        .ParseBlock<IdentifyTables>()
                        .Show(Color.Red)
                    .ParsePdf<PreProcessImages>()
                        .Show(Color.Green)
                        .ParseBlock<RemoveOverlapedImages>()
                    .ParsePdf<ProcessPdfText>()
                        .ParseBlock<RemoveSmallFonts>()
                        .ParseBlock<MergeTableText>()
                        .ParseBlock<HighlightTextTable>()
                        .ParseBlock<RemoveTableText>()
                        .ParseBlock<GroupLines>()
                            .Validate<RemoveHeaderImage>().ShowErrors(p => p.Show(Color.Purple))
                        .ParseBlock<RemoveHeaderImage>()

                        .ParseBlock<FindInitialBlocksetWithRewind>()
                        .ParseBlock<BreakColumnsLight>()
                            //.ParseBlock<BreakColumns>()
                            .Validate<RemoveFooter>().ShowErrors(p => p.Show(Color.Purple))
                            .ParseBlock<RemoveFooter>();
                    });

            pipeline.Done();
        }
        public static IEnumerable<TextLine> GetEnumerableLines(string basename)
        {
            var pipeline = new Execution.Pipeline();

            var result =
            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/{basename}-test-output.pdf")
                    .StreamConvert<CreateTextLines>(ProcessPage);                    

            return result;
        }

        public static PipelineText<TextLine> GetTextLines(string basename)
        {
            var pipeline = new Execution.Pipeline();

            var result =
            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/{basename}-test-output.pdf")
                    .AllPages<CreateTextLines>(ProcessPage);                  

            return result;
        }

        public static PipelineText<TextLine> GetTextLinesWithPipeline(string basename, out Execution.Pipeline pipeline)
        {
            pipeline = new Execution.Pipeline();

            var result =
            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/{basename}-test-output.pdf")
                    .AllPages<CreateTextLines>(ProcessPage);

            return result;
        }

        public static PipelineText<TextStructure> GetTextParagraphs(string basename)
        {
            var pipeline = new Execution.Pipeline();

            var result =
            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/{basename}-test-output.pdf")
                    .AllPages<CreateTextLines>(ProcessPage)
                    .ConvertText<CreateStructures, TextStructure>();

            return result;
        }

        public static void ProcessPage(PipelineInputPdf.PipelineInputPdfPage page)
        {
            page.ParsePdf<PreProcessTables>()
                        .ParseBlock<IdentifyTables>()
                    .ParsePdf<PreProcessImages>()
                            .Validate<RemoveOverlapedImages>().ShowErrors(p => p.Show(Color.Red))
                        .ParseBlock<RemoveOverlapedImages>()
                    .ParsePdf<ProcessPdfText>()
                        //.Validate<MergeTableText>().ShowErrors(p => p.Show(Color.Blue))
                        .ParseBlock<MergeTableText>()
                        //.Validate<HighlightTextTable>().ShowErrors(p => p.Show(Color.Green))
                        .ParseBlock<HighlightTextTable>()
                        .ParseBlock<RemoveTableText>()
                        .ParseBlock<GroupLines>()
                            .Show(Color.Orange)
                            .Validate<RemoveHeaderImage>().ShowErrors(p => p.Show(Color.Purple))
                            .ParseBlock<RemoveHeaderImage>()
                        .ParseBlock<FindInitialBlocksetWithRewind>()
                        .ParseBlock<BreakColumns>()
                            .Validate<RemoveFooter>().ShowErrors(p => p.Show(Color.Purple))
                            .ParseBlock<RemoveFooter>()
                        .ParseBlock<AddTableSpace>()
                        .ParseBlock<AddImageSpace>()
                        .ParseBlock<BreakInlineElements>()
                        .ParseBlock<ResizeBlocksets>()
                            .Validate<ResizeBlocksets>().ShowErrors(p => p.Show(Color.Red))
                        .ParseBlock<OrderBlocksets>()
                        .Show(Color.Orange)
                        .ShowLine(Color.Black);
        }

        public static void ProcessPipeline(string basename)
        {
            PdfReaderException.ContinueOnException();
            
            var artigos = GetTextLineFromPipeline(basename, out Execution.Pipeline pipeline)
                            .ConvertText<CreateTextLineIndex, TextLine>()
                            .ConvertText<CreateStructures, TextStructure>()
                                .ShowPdf<ShowStructureCentral>($"{basename}-show-central.pdf")
                            .ConvertText<CreateTextSegments, TextSegment>()
                            .ConvertText<CreateTreeSegments, TextSegment>()
                                .Log<AnalyzeTreeStructure>($"{basename}-tree.txt")
                                .ToList();

            Console.WriteLine($"FILENAME: {pipeline.Filename}");

            var validation = pipeline.Statistics.Calculate<ValidateFooter, StatsPageFooter>();
            var layout = (ValidateLayout)pipeline.Statistics.Calculate<ValidateLayout, StatsPageLayout>();
            var overlap = (ValidateOverlap)pipeline.Statistics.Calculate<ValidateOverlap, StatsBlocksOverlapped>();

            var pagesLayout = layout.GetPageErrors().ToList();
            var pagesOverlap = overlap.GetPageErrors().ToList();
            var pages = pagesLayout.Concat(pagesOverlap).Distinct().OrderBy(t => t).ToList();

            ExtractPages($"{basename}-parser-output", $"{basename}-page-errors-output", pages);

            pipeline.Done();
        }

        static PipelineText<TextLine> GetTextLineFromPipeline(string basename, out Execution.Pipeline pipeline)
        {
            pipeline = new Execution.Pipeline();

            var result =
            pipeline.Input($"{basename}.pdf")
                    .Output($"{basename}-parser-output.pdf")
                    .AllPagesExcept<CreateTextLines>(new int[] { }, page =>
                              page.ParsePdf<PreProcessTables>()
                                  .ParseBlock<IdentifyTables>()
                              //.Show(Color.Blue)
                              .ParsePdf<PreProcessImages>()
                                  .ParseBlock<BasicFirstPageStats>()
                                  //.Validate<RemoveOverlapedImages>().ShowErrors(p => p.Show(Color.Blue))
                                  .ParseBlock<RemoveOverlapedImages>()
                              .ParsePdf<ProcessPdfText>()
                                  .Validate<RemoveSmallFonts>().ShowErrors(p => p.ShowText(Color.Green))
                                  .ParseBlock<RemoveSmallFonts>()
                                  //.Validate<MergeTableText>().ShowErrors(p => p.Show(Color.Blue))
                                  .ParseBlock<MergeTableText>()
                                  //.Validate<HighlightTextTable>().ShowErrors(p => p.Show(Color.Green))
                                  .ParseBlock<HighlightTextTable>()
                                  .ParseBlock<RemoveTableText>()
                                  .ParseBlock<ReplaceCharacters>()
                                  .ParseBlock<GroupLines>()
                                  .ParseBlock<RemoveTableDotChar>()
                                      .Show(Color.Yellow)
                                      .Validate<RemoveHeaderImage>().ShowErrors(p => p.Show(Color.Purple))
                                  .ParseBlock<RemoveHeaderImage>()
                                  .ParseBlock<FindInitialBlocksetWithRewind>()
                                      .Show(Color.Gray)
                                  .ParseBlock<BreakColumnsLight>()
                                  //.ParseBlock<BreakColumns>()
                                  .ParseBlock<AddTableSpace>()
                                  .ParseBlock<RemoveTableOverImage>()
                                  .ParseBlock<RemoveImageTexts>()
                                  .ParseBlock<AddImageSpace>()
                                      .Validate<RemoveFooter>().ShowErrors(p => p.Show(Color.Purple))
                                      .ParseBlock<RemoveFooter>()
                                  .ParseBlock<AddTableLines>()

                                      // Try to rewrite column
                                      .ParseBlock<BreakColumnsRewrite>()

                                  .ParseBlock<BreakInlineElements>()
                                  .ParseBlock<ResizeBlocksets>()
                                  .ParseBlock<ResizeBlocksetMagins>()

                                    // Reorder the blocks
                                    .ParseBlock<OrderBlocksets>()

                                  .ParseBlock<OrganizePageLayout>()
                                  .ParseBlock<MergeSequentialLayout>()
                                  .ParseBlock<ResizeSequentialLayout>()

                                      .Show(Color.Orange)
                                      .ShowLine(Color.Black)

                                  .ParseBlock<CheckOverlap>()

                                      .Validate<CheckOverlap>().ShowErrors(p => p.Show(Color.Red))
                                      .Validate<ValidatePositiveCoordinates>().ShowErrors(p => p.Show(Color.Red))
                    );

            return result;
        }

        static void ExtractPages(string basename, string outputname, IList<int> pages)
        {
            using (var pipeline = new Execution.Pipeline())
            {
                pipeline.Input($"{basename}.pdf")
                        .ExtractPages($"{outputname}.pdf", pages);
            }
        }
    }
}
