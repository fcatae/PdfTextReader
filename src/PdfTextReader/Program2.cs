using iText.Kernel.Pdf;
using PdfTextReader.Execution;
using System;
using System.Drawing;
using System.IO;

namespace PdfTextReader
{
    class Program2
    {
        static void Main(string[] args)
        {
            var pipeline = new Pipeline();

            var lines = pipeline.GetLines("p40");

            //Testing paragraphs
            var process = new Structure.ProcessStructure2();
            var paragraphs = process.ProcessParagraph(lines);

            //Testing Naming Structures (e.g Title, Sector, etc)
            var parser = new Parser.ProcessParser();
            var contents = parser.ProcessStructures(paragraphs);
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
