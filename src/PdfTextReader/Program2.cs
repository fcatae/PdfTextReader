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

            ProcessSingle("p42");
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

        void TestPipeline()
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input("input") //.Output
                    .Page(1)  // .AllPages( p => p.CurrentPage )
                    .ParsePdf<PreProcessTables>()
                    .StoreResult("INLINETABLES")
                        .Output("table-output")
                        .Show<TableCell>(b => b.Op == 1, Color.Green)
                    .Output("lines")
                    .ParsePdf<ProcessPdfText>()
                    .ParseBlock<RemoveTableInlineText>() // "INLINETABLES"
                                                         //.ParseBlock<PreProcessTables>() // use singleton instead?
                    .ParseBlock<FindPageColumns>()
                    .ParseBlock<BreakColumns>()
                        .Validate<BreakColumns>(Color.Red)
                    .ParseBlock<RemoveHeader>().Debug(Color.Blue)
                    .ParseBlock<RemoveFooter>().Debug(Color.Blue)
                        .Validate<ValidFooter>(p => new Exception())
                    .ParseBlock<CreateLines>()
                    .Text<CreateStructures>()
                        .Show(Color.Yellow)
                    .ParseText<ProcessParagraphs>()
                    .ParseText<ProcessStructure>()
                        .Output("structures")
                        .Show(Color.Red)
                    .ParseContent<ProcessArticle>()
                        ;//.SaveXml(p => $"file-{p.page}");

        }
    }
}
