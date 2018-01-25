using System;
using System.Collections.Generic;
using System.Text;
using PdfTextReader.Base;
using PdfTextReader.Parser;
using PdfTextReader.ExecutionStats;
using PdfTextReader.TextStructures;
using PdfTextReader.Execution;
using PdfTextReader.PDFText;
using System.Drawing;
using PdfTextReader.PDFCore;
using System.Linq;

namespace PdfTextReader
{
    class Program3
    {        
        public static void ProcessStats2(string basename = "DO1_2017_01_06")
        {
            PdfReaderException.ContinueOnException();

            //PdfWriteText.Test();
            //return;
            Console.WriteLine();
            Console.WriteLine("Program3 - ProcessTextLines");
            Console.WriteLine();

            basename = ExtractPage(basename, 35);
            
            //ValidatorPipeline.Process("DO1_2010_02_10.pdf", @"c:\pdf\output_6", @"c:\pdf\valid");

            //Examples.FollowText(basename);
            //Examples.ShowHeaderFooter(basename);

            //Examples.ProcessPipeline("bin/"  + basename);

            var artigos = GetTextLinesWithPipelineBlockset(basename, out Execution.Pipeline pipeline)
                                //.Log<AnalyzeLines>(Console.Out)
                            .ConvertText<CreateTextLineIndex,TextLine>()
                            .ConvertText<CreateStructures, TextStructure>()
                                //.ShowPdf<ShowStructureCentral>($"bin/{basename}-show-central.pdf")
                                //.Log<AnalyzePageInfo<TextStructure>>(Console.Out)
                                //.Log<AnalyzeStructures>(Console.Out)
                                //.Log<AnalyzeStructuresCentral>($"bin/{basename}-central.txt")
                                //.PrintAnalytics($"bin/{basename}-print-analytics.txt")
                            .ConvertText<CreateTextSegments, TextSegment>()
                                //.Log<AnalyzeSegmentStats>($"bin/{basename}-segments-stats.txt")
                            .ConvertText<CreateTreeSegments, TextSegment>()
                                .Log<AnalyzeSegmentTitles>($"bin/{basename}-segment-titles-tree.txt")
                                .Log<AnalyzeTreeStructure>(Console.Out)
                            .ToList();
            
            Console.WriteLine($"FILENAME: {pipeline.Filename}");
            
            var validation = pipeline.Statistics.Calculate<ValidateFooter, StatsPageFooter>();
            var layout = (ValidateLayout)pipeline.Statistics.Calculate<ValidateLayout, StatsPageLayout>();
            var overlap = (ValidateOverlap)pipeline.Statistics.Calculate<ValidateOverlap, StatsBlocksOverlapped>();

            var pagesLayout = layout.GetPageErrors().ToList();
            var pagesOverlap = overlap.GetPageErrors().ToList();
            var pages = pagesLayout.Concat(pagesOverlap).Distinct().OrderBy(t =>t).ToList();

            //ExtractPages($"{basename}-parser-output", $"{basename}-page-errors-output", pages);
        }

        static PipelineText<TextLine> GetTextLinesWithPipelineBlockset(string basename, out Execution.Pipeline pipeline, int startPage=1)
        {
            if (startPage == 1)
                return GetTextLines(basename, out pipeline, new int[] { });

            var skipPages = Enumerable.Range(1, startPage - 1).ToArray();

            return GetTextLines(basename, out pipeline, skipPages);
        }

        static PipelineText<TextLine> GetTextLines(string basename, out Execution.Pipeline pipeline, int[] exceptPages)
        {
            pipeline = new Execution.Pipeline();

            var result =
            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/{basename}-parser-output.pdf")
                    .AllPagesExcept<CreateTextLines>(exceptPages, page =>
                              page.ParsePdf<PreProcessTables>()
                                  .ParseBlock<IdentifyTables>()
                              .ParsePdf<PreProcessImages>()
                                  .ParseBlock<BasicFirstPageStats>()
                                  //.Validate<RemoveOverlapedImages>().ShowErrors(p => p.Show(Color.Blue))
                                  .ParseBlock<RemoveOverlapedImages>()
                              .ParsePdf<ProcessPdfText>()
                                  //.Validate<RemoveSmallFonts>().ShowErrors(p => p.ShowText(Color.Green))
                                  .ParseBlock<RemoveSmallFonts>()
                                  //.Validate<MergeTableText>().ShowErrors(p => p.Show(Color.Blue))
                                  .ParseBlock<MergeTableText>()
                                  .Validate<HighlightTextTable>().ShowErrors(p => p.Show(Color.Green))
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
                                  .ParseBlock<AddTableHorizontalLines>()
                                  .ParseBlock<RemoveBackgroundNonText>()
                                  
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
                pipeline.Input($"bin/{basename}.pdf")
                        .ExtractPages($"bin/{outputname}.pdf", pages);
            }
        }

        static void ExtractPages(string basename, IList<int> pages)
        {
            foreach(var p in pages)
            {
                ExtractPages(basename, $"{basename}-p{p}", new int[] { p });
            }
        }
        static string ExtractPage(string basename, int p, bool create = true)
        {
            string outputname = $"{basename}-p{p}";

            if(create)
                ExtractPages(basename, outputname, new int[] { p });

            return outputname;
        }
    }
}
