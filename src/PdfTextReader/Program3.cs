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

            //ExtractPages(basename, $"{basename}-p2345", new int[] { 2, 3, 4, 5 });
            // ExtractPages(basename, $"{basename}-ps", new int[] { 10, 32, 34, 35, 37, 40, 42, 60, 80 });

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
                                //.Log<AnalyzeSegmentTitles>($"bin/{basename}-tree.txt")
                                //.Log<AnalyzeSegmentStats>($"bin/{basename}-segments-stats.txt")
                            .ConvertText<CreateTreeSegments, TextSegment>()
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

        static PipelineText<TextLine> GetTextLinesWithPipelineBlockset(string basename, out Execution.Pipeline pipeline)
        {
            pipeline = new Execution.Pipeline();

            var result =
            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/{basename}-parser-output.pdf")
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
                                    .Show(Color.LightGray)
                                      .Validate<RemoveFooter>().ShowErrors(p => p.Show(Color.Purple))
                                      .ParseBlock<RemoveFooter>()
                                   
                                      // Try to rewrite column
                                      .ParseBlock<BreakColumnsRewrite>()

                                  .ParseBlock<BreakInlineElements>()
                                  .ParseBlock<ResizeBlocksets>()
                                  .ParseBlock<ResizeBlocksetMagins>()
                                    
                                    // Reorder the blocks
                                    .ParseBlock<OrderBlocksets>()
                                  
                                      .Show(Color.Orange)
                                  .ShowLine(Color.Black)
                                  .ParseBlock<OrganizePageLayout>()
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
    }
}
