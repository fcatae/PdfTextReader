using iText.Kernel.Pdf;
using PdfTextReader.Execution;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using PdfTextReader.Base;
using PdfTextReader.Parser;
using PdfTextReader.ExecutionStats;
using PdfTextReader.TextStructures;

namespace PdfTextReader
{
    class Program2
    {
        public static void MainTest()
        {
            ProcessSingleTextWithAnalytics("p40");
        }
        
        static void ProcessSingleText(string page)
        {
            var artigos = Examples.GetTextParagraphs(page)
                            .ConvertText<TransformExemplo, TextStructure>()
                            .ConvertText<TransformArtigo, Artigo>()
                            .ToList();

            var procParser = new ProcessParser();
            procParser.XMLWriter(artigos, $"bin/{page}-out");
        }
        static void ProcessSingleTextWithAnalytics(string page)
        {
            var artigos = Examples.GetTextLines(page)
                            // show text lines
                            .Process( PrintAnalytics.ShowTextLine($"bin/{page}-out-txtline.xml") )
                            .Process2(new PrintAnalytics2.ShowLines($"bin/{page}-out-txtline2.xml"))

                            // TextLine -> TextStructure
                            .ConvertText<CreateParagraphs, TextStructure>()                            

                            // show text structures
                            .Process( PrintAnalytics.ShowTextStructure($"bin/{page}-out-txtstr.xml") )
                            .Process2(new PrintAnalytics2.ShowStructures($"bin/{page}-out-txtstr2.xml"))

                            // convert to artigos
                            .ConvertText<TransformArtigo, Artigo>()

                            // array
                            .ToList();

            var procParser = new ProcessParser();
            procParser.XMLWriter(artigos, $"bin/{page}-out");
        }

        static void ProcessSingle(string page)
        {
            var lines = Examples.GetEnumerableLines(page);

            //Analyzing Grid Font
            ExecutionStats.ProcessStats.SetGridStyle(ExecutionStats.ProcessStats.GetAllTextInfo(lines));

            //Testing paragraphs
            var process = new TextStructures.ProcessStructure2();
            var paragraphs = process.ProcessParagraph(lines);

            //Testing Naming Structures (e.g Title, Sector, etc)
            var parser = new Parser.ProcessParser();
            var contents = parser.ProcessStructures(paragraphs);

            PrintAnalyticsFunc(page, lines, paragraphs, contents);
        }

        static void ProcessBatch(string subfolder)
        {
            var dir = new DirectoryInfo($"bin/{subfolder}");
            var pipeline = new ExamplesPipeline();

            foreach (var f in dir.EnumerateFiles("*.pdf"))
            {
                string filename = f.Name;
                string basename = Path.GetFileNameWithoutExtension(filename);

                var lines = Examples.GetEnumerableLines($"/{subfolder}/{basename}");

                //Analyzing Grid Font
                ExecutionStats.ProcessStats.SetGridStyle(ExecutionStats.ProcessStats.GetAllTextInfo(lines));

                //Testing paragraphs
                var process = new TextStructures.ProcessStructure2();
                var paragraphs = process.ProcessParagraph(lines);

                //Testing Naming Structures (e.g Title, Sector, etc)
                var parser = new Parser.ProcessParser();
                var contents = parser.ProcessStructures(paragraphs);

                PrintAnalyticsFunc(basename, lines, paragraphs, contents);

            }
        }

        static void PrintAnalyticsFunc(string pdfname, IEnumerable<TextLine> lines, IEnumerable<TextStructure> structures, IEnumerable<Parser.Conteudo> contents)
        {
            ExecutionStats.ProcessStats.PrintAnalytics(pdfname, lines, structures, contents);
        }


        //ProcessBatch("dz");

        //ExamplesPipeline.MultipageCreateArtigos("pg5");

        //ExamplesPipeline.ShowTablesImages("DO1_2016_12_20-p23");

        // ExamplesPipeline.MultipageCreateArtigos("p40");

            //ExamplesPipeline.MultipageCreateArtigos("pg20");

            //ExamplesPipeline.RunCorePdf("pg5");

            //ExamplesPipeline.ExtractPage("DO1_2016_12_20", 75);
            //ExamplesPipeline.ExtractPage("DO1_2016_12_20", 36);
            //ExamplesPipeline.ExtractPage("DO1_2016_12_20", 23);
            //ExamplesPipeline.ExtractPage("DO1_2016_12_20", 80);
    }
}
