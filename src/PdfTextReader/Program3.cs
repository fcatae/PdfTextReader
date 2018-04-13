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
        // public static void CreateLayout(string basename)
        // {
        //     //PipelineInputPdf.StopOnException();
        //     //PdfReaderException.ContinueOnException();

        //     Console.WriteLine();
        //     Console.WriteLine("Program3 - CreateLayout");
        //     Console.WriteLine();

        //     var textLines = GetTextLines(basename, out Execution.Pipeline pipeline)
        //                     .ConvertText<CreateStructures, TextStructure>()
        //                     .ConvertText<CreateTextSegments, TextSegment>()
        //                     .ToList();

        //     Console.WriteLine($"FILENAME: {pipeline.Filename}");

        //     var statistics = pipeline.Statistics;
        //     //var layout = (ValidateLayout)statistics.Calculate<ValidateLayout, StatsPageLayout>();
        //     var layout = statistics.RetrieveStatistics<StatsPageLayout>();

        //     // pipeline.Statistics.SaveStats<StatsPageLayout>($"bin/{basename}-pagelayout.txt");

        //     pipeline.ExtractOutput<ShowParserWarnings>($"bin/{basename}-parser-errors.pdf");
        // }


        public static void ProcessStage(string basename)
        {
            Console.WriteLine();
            Console.WriteLine("ProcessStage");
            Console.WriteLine();

            PipelineInputPdf.StopOnException();

            using (var context = new ParserStages.StageContext(basename))
            {
                var stage0 = new ParserStages.StagePdfInput(context);
                stage0.Process();

                var stage1 = new ParserStages.StagePageMargins(context);
                stage1.Process();
            }
        }

        public static void ProcessStats2(string basename = "DO1_2017_01_06", int page=-1)
        {
            //PipelineInputPdf.StopOnException();
            //PdfReaderException.ContinueOnException();
            
            Console.WriteLine();
            Console.WriteLine("Program3 - ProcessStats2");
            Console.WriteLine();

            if( page != -1 )
            {
                basename = ExtractPage(basename, page);
            }

            var conteudo = GetTextLines(basename, out Execution.Pipeline pipeline)
                                .Log<AnalyzeLines>($"bin/{basename}-lines.txt")
                            .ConvertText<CreateTextLineIndex,TextLine>()
                            .ConvertText<PreCreateStructures, TextLine2>()
                            .ConvertText<CreateStructures2, TextStructure>()
                            .ConvertText<PreCreateTextSegments, TextStructureAgg>()
                            .ConvertText<AggregateStructures, TextStructure>()
                                .ShowPdf<ShowStructureCentral>($"bin/{basename}-show-central.pdf")
                                //.Log<AnalyzePageInfo<TextStructure>>(Console.Out)
                                .Log<AnalyzeStructures>($"bin/{basename}-struct.txt")
                                .Log<AnalyzeStructuresCentral>($"bin/{basename}-central.txt")
                                //.PrintAnalytics($"bin/{basename}-print-analytics.txt")
                            .ConvertText<CreateTextSegments, TextSegment>()
                                //.Log<AnalyzeSegmentStats>($"bin/{basename}-segments-stats.txt")
                            .ConvertText<CreateTreeSegments, TextSegment>()
                                .Log<AnalyzeSegmentTitles>($"bin/{basename}-segment-titles-tree.txt")
                                .Log<AnalyzeTreeStructure>(Console.Out)
                            .ConvertText<TransformConteudo, Conteudo>()
                            .ToList();

            Console.WriteLine($"FILENAME: {pipeline.Filename}");
            
            var createArticle = new TransformArtigo();
            var artigos = createArticle.Create(conteudo);
            createArticle.CreateXML(artigos, $"bin/{basename}", basename);

            pipeline.ExtractOutput<ShowParserWarnings>($"bin/{basename}-parser-errors.pdf");
        }

        static PipelineText<TextLine> GetTextLines(string basename, out Execution.Pipeline pipeline)
        {
            pipeline = new Execution.Pipeline();

            var result =
            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/{basename}-parser-output.pdf")
                    .AllPages<CreateTextLines>(page =>
                              page.ParsePdf<PreProcessTables>()
                                  .ParseBlock<IdentifyTables>()             // 1
                              .ParsePdf<PreProcessImages>()
                                  .ParseBlock<BasicFirstPageStats>()        // 2
                                  //.Validate<RemoveOverlapedImages>().ShowErrors(p => p.Show(Color.Blue))
                                  .ParseBlock<RemoveOverlapedImages>()      // 3
                              .ParsePdf<ProcessPdfText>()                   // 4
                                  //.Validate<RemoveSmallFonts>().ShowErrors(p => p.ShowText(Color.Green))
                                    .Validate<HideSmallFonts>().ShowErrors(p => p.Show(Color.Green))
                                  .ParseBlock<HideSmallFonts>()           // 5
                        //.ParseBlock<RemoveSmallFonts>()           // 5
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
