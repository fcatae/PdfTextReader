using iText.Kernel.Pdf;
using System;
using System.Collections.Generic;
using System.IO;

namespace PdfTextReader
{
    class Program2
    {
        static void Main(string[] args)
        {
            //ProcessBatch("dz");

            ProcessSingle("p44");
        }

        static void ProcessSingle(string page)
        {
            var pipeline = new Pipeline();

            var lines = pipeline.GetLines(page);

            //Testing paragraphs
            var process = new Structure.ProcessStructure2();
            var paragraphs = process.ProcessParagraph(lines);

            //Testing Naming Structures (e.g Title, Sector, etc)
            var parser = new Parser.ProcessParser();
            var contents = parser.ProcessStructures(paragraphs);

            PrintAnalytics(page, lines, paragraphs, contents);
        }

        static void ProcessBatch(string subfolder)
        {
            var dir = new DirectoryInfo($"bin/{subfolder}");
            var pipeline = new Pipeline();

            foreach (var f in dir.EnumerateFiles("*.pdf"))
            {
                string filename = f.Name;
                string basename = Path.GetFileNameWithoutExtension(filename);

                var lines = pipeline.GetLines($"/{subfolder}/{basename}");

                //Testing paragraphs
                var process = new Structure.ProcessStructure2();
                var paragraphs = process.ProcessParagraph(lines);

                //Testing Naming Structures (e.g Title, Sector, etc)
                var parser = new Parser.ProcessParser();
                var contents = parser.ProcessStructures(paragraphs);

                PrintAnalytics(basename, lines, paragraphs, contents);

            }
        }

        static void PrintAnalytics(string pdfname, IEnumerable<Structure.TextLine> lines, List<Structure.TextStructure> structures, List<Parser.TextContent> contents)
        {
            Stats.ProcessStats stats = new Stats.ProcessStats();
            stats.PrintAnalytics(pdfname, lines, structures, contents);
        }
    }
}
