using iText.Kernel.Pdf;
using PdfTextReader.Execution;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace PdfTextReader
{
    class Program2
    {
        static void Main(string[] args)
        {
            Program2.MainTest();
        }

        public static void MainTest()
        {
            //ProcessBatch("dz");

            //Examples.MultipageCreateArtigos("pg5");

            //Examples.ShowTablesImages("DO1_2016_12_20-p23");

            //Examples.MultipageCreateArtigos("p40");

            //Examples.MultipageCreateArtigos("pg20");

            //Examples.RunCorePdf("pg5");

            //Examples.ExtractPage("DO1_2016_12_20", 75);
            //Examples.ExtractPage("DO1_2016_12_20", 36);
            //Examples.ExtractPage("DO1_2016_12_20", 23);
            //Examples.ExtractPage("DO1_2016_12_20", 80);
        }

        static void ProcessSingle(string page)
        {
            var lines = Examples.GetLinesUsingPipeline(page);

            //Analyzing Grid Font
            ExecutionStats.ProcessStats.SetGridStyle(ExecutionStats.ProcessStats.GetAllTextInfo(lines));

            //Testing paragraphs
            var process = new TextStructures.ProcessStructure2();
            var paragraphs = process.ProcessParagraph(lines);

            //Testing Naming Structures (e.g Title, Sector, etc)
            var parser = new Parser.ProcessParser();
            var contents = parser.ProcessStructures(paragraphs);

            PrintAnalytics(page, lines, paragraphs, contents);
        }

        static void ProcessBatch(string subfolder)
        {
            var dir = new DirectoryInfo($"bin/{subfolder}");
            var pipeline = new Examples();

            foreach (var f in dir.EnumerateFiles("*.pdf"))
            {
                string filename = f.Name;
                string basename = Path.GetFileNameWithoutExtension(filename);

                var lines = Examples.GetLinesUsingPipeline($"/{subfolder}/{basename}");

                //Analyzing Grid Font
                ExecutionStats.ProcessStats.SetGridStyle(ExecutionStats.ProcessStats.GetAllTextInfo(lines));

                //Testing paragraphs
                var process = new TextStructures.ProcessStructure2();
                var paragraphs = process.ProcessParagraph(lines);

                //Testing Naming Structures (e.g Title, Sector, etc)
                var parser = new Parser.ProcessParser();
                var contents = parser.ProcessStructures(paragraphs);

                PrintAnalytics(basename, lines, paragraphs, contents);

            }
        }

        static void PrintAnalytics(string pdfname, IEnumerable<TextStructures.TextLine> lines, IEnumerable<TextStructures.TextStructure> structures, IEnumerable<Parser.Conteudo> contents)
        {
            ExecutionStats.ProcessStats.PrintAnalytics(pdfname, lines, structures, contents);
        }
                
    }
}
