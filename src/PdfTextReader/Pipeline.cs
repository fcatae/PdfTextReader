using PdfTextReader.Execution;
using PdfTextReader.PDFCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PdfTextReader
{
    class Pipeline
    {

        public static void TestPipeline(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf") //.Output
                    .Page(1)  // .AllPages( p => p.CurrentPage )
                    //.ParsePdf<PreProcessTables>()
                    //    .Output($"bin/{basename}-table-output.pdf")
                    //    .DebugBreak( c => c != null )
                    //    .Show(Color.Green)
                    //.Show<TableCell>(b => b.Op == 1, Color.Green)
                    //.Output("lines")
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
                        // .Show(Color.Yellow)
                    .ParseText<ProcessParagraphs>()
                    .ParseText<ProcessStructure>()
                        // .Output("structures")
                        // .Show(Color.Red)
                    .ParseContent<ProcessArticle>()
                        ;//.SaveXml(p => $"file-{p.page}");

        }

        // Pipeline Definition: 
        //   * TableListener -> ProcessTable
        //   * TextListener  -> Remove Tables -> ProcessColumns -> BreakColumns -> MergeBlocks
        //
        public IEnumerable<Structure.TextLine> GetLines(string basename)
        {
            var user = new UserWriter2();

            // run the listener 1
            // process blocksets
            user.ProcessBlockExtra($"bin/{basename}.pdf", $"bin/{basename}-table-output.pdf");

            var tablesFound = user.ActiveTables;

            // run the listener 2
            // process blocksets
            user.ProcessBlock($"bin/{basename}.pdf", $"bin/{basename}-output.pdf");

            var texts = user.ActiveTexts;

            // TODO: transform into blocklines
            var proc = new PDFCore.ProcessBlockLines();
            var blocks = proc.FindLines(texts);

            var all_lines = new List<Structure.TextLine>();
            // transform into textLines
            foreach(var bset in blocks)
            {
                var proc2 = new Structure.ProcessStructure();
                var lines = proc2.ProcessLine(bset);

                all_lines.AddRange(lines);
            }

            return all_lines;
        }
    }
}
