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
            //ProcessBatch("dz");

            Pipeline.RemoveOverlapedImages("dz-easy/dz079");

            //Pipeline.TestEnumFiles("bin/dz-easy", f => Pipeline.RunCorePdf(f.Substring(4)) );

            //ProcessSingle("p42");

            //var lines1 = Pipeline.GetLinesUsingPipeline("p40");

            //var pipeline = new Pipeline();

            //var lines2 = pipeline.GetLines("p40");


        }

        static void ProcessSingle(string page)
        {
            var pipeline = new Pipeline();

            var lines = pipeline.GetLines(page);

            //Analyzing Grid Font
            Stats.ProcessStats.SetGridStyle(Stats.ProcessStats.GetAllTextInfo(lines));

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

                //Analyzing Grid Font
                Stats.ProcessStats.SetGridStyle(Stats.ProcessStats.GetAllTextInfo(lines));

                //Testing paragraphs
                var process = new Structure.ProcessStructure2();
                var paragraphs = process.ProcessParagraph(lines);

                //Testing Naming Structures (e.g Title, Sector, etc)
                var parser = new Parser.ProcessParser();
                var contents = parser.ProcessStructures(paragraphs);

                PrintAnalytics(basename, lines, paragraphs, contents);

            }
        }

        static void PrintAnalytics(string pdfname, IEnumerable<Structure.TextLine> lines, IEnumerable<Structure.TextStructure> structures, IEnumerable<Parser.Conteudo> contents)
        {
            Stats.ProcessStats.PrintAnalytics(pdfname, lines, structures, contents);
        }
                
    }
}
