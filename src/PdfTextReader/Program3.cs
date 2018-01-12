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
        public static void ProcessStats()
        {
            Console.WriteLine();
            Console.WriteLine("Program3 - ProcessTextLines");
            Console.WriteLine();

            string basename = "dou555";

            //Extract(21);

            Examples.FollowText(basename);

            var artigos = GetTextLinesWithPipelineBlockset(basename, out Execution.Pipeline pipeline)
                                .Log<AnalyzeLines>(Console.Out)
                            .ConvertText<CreateStructures, TextStructure>()
                                //.PrintAnalytics($"bin/{basename}-print-analytics.txt")
                                //.Log<AnalyzeStructures>(Console.Out)
                            .ConvertText<CreateTextSegments, TextSegment>()
                                .Log<AnalyzeSegmentTitles>($"bin/{basename}-tree.txt")
                                .Log<AnalyzeSegmentStats>($"bin/{basename}-segments-stats.txt")
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
                    .AllPagesExcept<CreateTextLines>(new int[] { 19,21,75},page =>
                            page.ParsePdf<PreProcessTables>()
                                .ParseBlock<IdentifyTables>()
                            .ParsePdf<PreProcessImages>()
                                    .Validate<RemoveOverlapedImages>().ShowErrors(p => p.Show(Color.Red))
                                .ParseBlock<RemoveOverlapedImages>()
                            .ParsePdf<ProcessPdfText>()
                                .Validate<RemoveSmallFonts>().ShowErrors(p => p.Show(Color.Green))
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
                            .ConvertText<TransformArtigo, Artigo>()
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
                            .ConvertText<TransformArtigo, Artigo>()
                                .DebugPrint()
                            .ToList();            

            var procParser = new ProcessParser();
            procParser.XMLWriterMultiple(artigos, $"bin/{basename}/{basename}-artigo");
        }

        public static void Extract(int page)
        {
            Console.WriteLine();
            Console.WriteLine("Program3 - Extract");
            Console.WriteLine();

            string basename = "dou555";

            ExamplesPipeline.ExtractPage(basename, page);

            //ExamplesPipeline.Extract(basename, 1);
            //ExamplesPipeline.Extract(basename, 5);
            //ExamplesPipeline.Extract(basename, 10);
            //ExamplesPipeline.Extract(basename, 20);
            //ExamplesPipeline.Extract(basename, 50);
        }
    }
}
