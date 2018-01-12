using System;
using System.Collections.Generic;
using System.Text;
using PdfTextReader.Base;
using PdfTextReader.Parser;
using PdfTextReader.ExecutionStats;
using PdfTextReader.TextStructures;

namespace PdfTextReader
{
    class Program3
    {
        public static void ProcessTextLines()
        {
            Console.WriteLine();
            Console.WriteLine("Program3 - ProcessTextLines");
            Console.WriteLine();

            string basename = "dou555-p1";

            var artigos = Examples.GetTextLines(basename)
                            .ConvertText<CreateStructures, TextStructure>()
                            .ConvertText<CreateTextSegments, TextSegment>()
                            .ConvertText<CreateTreeSegments, TextSegment>()
                                .Log<AnalyzeSegmentTitles>($"bin/{basename}-tree.txt")
                                .Log<AnalyzeSegmentStats>($"bin/{basename}-segments-stats.txt")
                            .ToList();
        }
        public static void ProcessStats()
        {
            Console.WriteLine();
            Console.WriteLine("Program3 - ProcessTextLines");
            Console.WriteLine();

            string basename = "dou555-p1";

            var artigos = Examples.GetTextLinesWithPipelineBlockset(basename, out Execution.Pipeline pipeline)
                            .ConvertText<CreateStructures, TextStructure>()
                                .PrintAnalytics($"bin/{basename}-print-analytics.txt")
                            .ConvertText<CreateTextSegments, TextSegment>()
                                .DebugPrint()
                                .Log<AnalyzeSegmentTitles>($"bin/{basename}-tree.txt")
                                .Log<AnalyzeSegmentStats>($"bin/{basename}-segments-stats.txt")
                            .ConvertText<CreateTreeSegments, TextSegment>()
                            .ToList();

            var validation = pipeline.Statistics.Calculate<ValidateFooter, StatsPageFooter>();
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

        public static void Extract()
        {
            Console.WriteLine();
            Console.WriteLine("Program3 - Extract");
            Console.WriteLine();

            string basename = "dou555";

            ExamplesPipeline.Extract(basename, 1);
            ExamplesPipeline.Extract(basename, 5);
            ExamplesPipeline.Extract(basename, 10);
            ExamplesPipeline.Extract(basename, 20);
            ExamplesPipeline.Extract(basename, 50);
        }
    }
}
