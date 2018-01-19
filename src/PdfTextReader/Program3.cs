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

namespace PdfTextReader
{
    class Program3
    {
        public static void ProcessWork()
        {
            //ExamplesWork.Blocks("p40");
            //ExamplesWork.BlockLines("p40");
            //ExamplesWork.BlockSets("p40");
            //ExamplesWork.BlockSets("dou555-p1"); 
            //ExamplesWork.FollowLine("p40");
            //ExamplesWork.FollowLine("dou555-p1");
            //ExamplesWork.BreakColuc:mn("p40");
            //ExamplesWork.BreakColumn("dou555-p1");
            //ExamplesWork.Tables("DO1_2016_12_20-p75");
            //ExamplesWork.Tables("dz088");
            //ExamplesWork.FindTables("DO1_2016_12_20-p75");
            //ExamplesWork.FindTables("dz088");
            //ExamplesWork.BadOrder("p40"); 
            //ExamplesWork.CorrectOrder("p40");
            //ExamplesWork.FindIds("dou555-p10");
            //ExamplesWork.ParseFinal("dou555-p10");
        }

        public static void ProcessStats()
       {
            var folders = new Execution.Pipeline();
            folders.EnumFolders(@"c:\pdf\2017", f => {
                ProcessStatsManyFolders(f);
            });
        }

        public static void ProcessStats2(string basename = "DO1_2017_01_06")
        {
            PdfReaderException.ContinueOnException();
            Examples.ShowTables(basename);

            //PdfWriteText.Test();
            //return;
            Console.WriteLine();
            Console.WriteLine("Program3 - ProcessTextLines");
            Console.WriteLine();

            // Extract(1);

            //Examples.FollowText(basename);
            //Examples.ShowHeaderFooter(basename);

            var artigos = GetTextLinesWithPipelineBlockset(basename, out Execution.Pipeline pipeline)
                            //.Log<AnalyzeLines>(Console.Out)
                            .ConvertText<CreateStructures, TextStructure>()
                            //.Log<AnalyzeStructures>(Console.Out)
                            //.Log<AnalyzeStructuresCentral>($"bin/{basename}-central.txt")
                            //.PrintAnalytics($"bin/{basename}-print-analytics.txt")
                            .ConvertText<CreateTextSegments, TextSegment>()
                            //.Log<AnalyzeSegmentTitles>($"bin/{basename}-tree.txt")
                            //.Log<AnalyzeSegmentStats>($"bin/{basename}-segments-stats.txt")
                            .ConvertText<CreateTreeSegments, TextSegment>()
                            .ToList();

            var validation = pipeline.Statistics.Calculate<ValidateFooter, StatsPageFooter>();
        }


        static PipelineText<TextLine> GetTextLinesWithPipelineBlockset(string basename, out Execution.Pipeline pipeline)
        {
            pipeline = new Execution.Pipeline();

            var result =
            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/{basename}-test-output.pdf")
                    .AllPagesExcept<CreateTextLines>(new int[] { }, page =>
                              page.ParsePdf<PreProcessTables>()
                                  .ParseBlock<IdentifyTables>()
                              .ParsePdf<PreProcessImages>()
                                      .Validate<RemoveOverlapedImages>().ShowErrors(p => p.Show(Color.Red))
                                  .ParseBlock<RemoveOverlapedImages>()
                              .ParsePdf<ProcessPdfText>()
                                  .Validate<RemoveSmallFonts>().ShowErrors(p => p.ShowText(Color.Green))
                                  .ParseBlock<RemoveSmallFonts>()
                                  //.Validate<MergeTableText>().ShowErrors(p => p.Show(Color.Blue))
                                  .ParseBlock<MergeTableText>()
                                  //.Validate<HighlightTextTable>().ShowErrors(p => p.Show(Color.Green))
                                  .ParseBlock<HighlightTextTable>()
                                  .ParseBlock<RemoveTableText>()
                                  .ParseBlock<GroupLines>()
                                      .Show(Color.Yellow)
                                      .Validate<RemoveHeaderImage>().ShowErrors(p => p.Show(Color.Purple))
                                  .ParseBlock<RemoveHeaderImage>()

                                  .ParseBlock<FindInitialBlocksetWithRewind>()
                                      .Show(Color.Gray)
                                  .ParseBlock<BreakColumnsLight>()
                                      //.ParseBlock<BreakColumns>()
                                      .Validate<RemoveFooter>().ShowErrors(p => p.Show(Color.Purple))
                                      .ParseBlock<RemoveFooter>()
                                  .ParseBlock<AddTableSpace>()
                                  .ParseBlock<AddImageSpace>()
                                  .ParseBlock<BreakInlineElements>()
                                  .ParseBlock<ResizeBlocksets>()
                                      .Validate<ResizeBlocksets>().ShowErrors(p => p.Show(Color.Red))
                                  .ParseBlock<OrderBlocksets>()
                                  .Show(Color.Orange)
                                  .ShowLine(Color.Black)
                    );

            return result;
        }

        public static void ProcessStatsManyFolders(string basename)
        {
            Console.WriteLine();
            Console.WriteLine("Program3 - ProcessStatsManyFolders");
            Console.WriteLine();
            
            var artigos = GetTextManyFolders(basename, out Execution.Pipeline pipeline)
                            .ConvertText<CreateStructures, TextStructure>()
                            .ConvertText<CreateTextSegments, TextSegment>()
                            .ConvertText<CreateTreeSegments, TextSegment>()
                            .ToList();

            pipeline.Done();

            //var validation = pipeline.Statistics.Calculate<ValidateFooter, StatsPageFooter>();
        }


        static PipelineText<TextLine> GetTextManyFolders(string basename, out Execution.Pipeline pipeline)
        {
            pipeline = new Execution.Pipeline();

            var result =
            pipeline.Input($"{basename}.pdf")
                    .Output($"{basename}-output.pdf")
                    .AllPagesExcept<CreateTextLines>(new int[] { }, page =>
                              page.ParsePdf<PreProcessTables>()
                                  .ParseBlock<IdentifyTables>()
                              .ParsePdf<PreProcessImages>()
                                      .Validate<RemoveOverlapedImages>().ShowErrors(p => p.Show(Color.Red))
                                  .ParseBlock<RemoveOverlapedImages>()
                              .ParsePdf<ProcessPdfText>()
                                  .Validate<RemoveSmallFonts>().ShowErrors(p => p.ShowText(Color.Green))
                                  .ParseBlock<RemoveSmallFonts>()
                                  //.Validate<MergeTableText>().ShowErrors(p => p.Show(Color.Blue))
                                  .ParseBlock<MergeTableText>()
                                  //.Validate<HighlightTextTable>().ShowErrors(p => p.Show(Color.Green))
                                  .ParseBlock<HighlightTextTable>()
                                  .ParseBlock<RemoveTableText>()
                                  .ParseBlock<GroupLines>()
                                      .Show(Color.Yellow)
                                      .Validate<RemoveHeaderImage>().ShowErrors(p => p.Show(Color.Purple))
                                  .ParseBlock<RemoveHeaderImage>()

                                  .ParseBlock<FindInitialBlocksetWithRewind>()
                                      .Show(Color.Gray)
                                  .ParseBlock<BreakColumnsLight>()
                                      //.ParseBlock<BreakColumns>()
                                      .Validate<RemoveFooter>().ShowErrors(p => p.Show(Color.Purple))
                                      .ParseBlock<RemoveFooter>()
                                  .ParseBlock<AddTableSpace>()
                                  .ParseBlock<AddImageSpace>()
                                  .ParseBlock<BreakInlineElements>()
                                  .ParseBlock<ResizeBlocksets>()
                                      .Validate<ResizeBlocksets>().ShowErrors(p => p.Show(Color.Red))
                                  .ParseBlock<OrderBlocksets>()
                                  .Show(Color.Orange)
                                  .ShowLine(Color.Black)
                    );

            return result;
        }

        public static void TesteArtigo()
        {
            Console.WriteLine();
            Console.WriteLine("Program3 - TesteArtigo");
            Console.WriteLine();

            string basename = "p40";

            var artigos = Examples.GetTextLines(basename)
                            .ConvertText<CreateStructures, TextStructure>()
                            .ConvertText<TransformArtigo, Conteudo>()
                            .ToList();
        }

        public static void SaveXml()
        {
            Console.WriteLine();
            Console.WriteLine("Program3 - SaveXml");
            Console.WriteLine();

            string basename = "pgfull";

            var artigos = Examples.GetTextLines(basename)
                            .ConvertText<CreateStructures, TextStructure>()
                            .ConvertText<TransformArtigo, Conteudo>()
                                .DebugPrint()
                            .ToList();            

            //var procParser = new ProcessParser();
            //procParser.XMLWriterMultiple(artigos, $"bin/{basename}/{basename}-artigo");
        }

        public static void Extract(int page)
        {
            Console.WriteLine();
            Console.WriteLine("Program3 - Extract");
            Console.WriteLine();

            string basename = "dou1212";

            //ExamplesPipeline.ExtractPage(basename, page);

            ExamplesPipeline.Extract(basename, 1);
            ExamplesPipeline.Extract(basename, 5);
            //ExamplesPipeline.Extract(basename, 10);
            //ExamplesPipeline.Extract(basename, 20);
            //ExamplesPipeline.Extract(basename, 50);
        }
    }
}
