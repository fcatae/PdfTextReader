using PdfTextReader.Execution;
using PdfTextReader.PDFCore;
using PdfTextReader.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PdfTextReader
{
    class Pipeline
    {
        public static void MarkAllComponents(string basename)
        {
            var pipeline = new Execution.Pipeline();
            
            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/{basename}-tmp-output.pdf")
                    .Page(1)
                    .ParsePdf<ProcessPdfText>()
                    .Show(Color.Yellow);

            pipeline.Done();            
        }

        public static void FollowText(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/{basename}-tmp-output.pdf")
                    .Page(1)
                    .ParsePdf<ProcessPdfText>()
                    .ShowLine(Color.Orange);

            pipeline.Done();
        }

        public static void ShowTables(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/{basename}-table-output.pdf")
                    .Page(1)
                    .ParsePdf<PreProcessTables>()
                    .Show(Color.Yellow)
                    .ParseBlock<IdentifyTables>()
                    .Show(Color.Green);

                    //.Show<TableCell>(b => b.Op == 1, Color.Green);

            pipeline.Done();
        }

        public static void ShowRenderPath(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/{basename}-tmp-output.pdf")
                    .Page(1)
                    .ParsePdf<PreProcessRenderPath>()
                    .ShowLine(Color.Green);                    

            pipeline.Done();
        }
        public static void GroupLines(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf").Page(1)
                    .Output($"bin/{basename}-tmp-output.pdf")                    
                    .ParsePdf<ProcessPdfText>()
                    .ParseBlock<GroupLines>()
                    .Show(Color.Orange);

            pipeline.Done();
        }

        public static void FindInitialBlockset(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf").Page(1)
                    .Output($"bin/{basename}-tmp-output.pdf")
                    .ParsePdf<ProcessPdfText>()
                    .ParseBlock<GroupLines>()
                    .ParseBlock<FindInitialBlockset>()
                    .Show(Color.Orange)
                    .ShowLine(Color.Gray);

            pipeline.Done();
        }

        public static void ValidateBreakColumns(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf").Page(1)
                    .Output($"bin/{basename}-tmp-output.pdf")
                    .ParsePdf<ProcessPdfText>()
                    .ParseBlock<GroupLines>()
                    .ParseBlock<FindInitialBlockset>()
                    .Validate<BreakColumns>()
                    .ShowErrors(p => p.Show(Color.Purple));

            pipeline.Done();
        }

        public static void BreakColumns(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf").Page(1)
                    .Output($"bin/{basename}-tmp-output.pdf")
                    .ParsePdf<ProcessPdfText>()
                    .ParseBlock<GroupLines>()
                    .ParseBlock<FindInitialBlockset>()
                        .Validate<BreakColumns>().ShowErrors(p => p.Show(Color.LightGray))
                        .ParseBlock<BreakColumns>()
                        .Show(Color.Green)
                        .Validate<BreakColumns>().ShowErrors(p => p.Show(Color.Red));

            pipeline.Done();
        }

        public static void RemoveHeaderFooter(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf").Page(1)
                    .Output($"bin/{basename}-tmp-output.pdf")
                    .ParsePdf<ProcessPdfText>()
                    .ParseBlock<GroupLines>()
                    .ParseBlock<FindInitialBlockset>()
                    .ParseBlock<BreakColumns>()
                    .Validate<RemoveFooter>().ShowErrors(p => p.Show(Color.Purple))
                    .Validate<RemoveHeader>().ShowErrors(p => p.Show(Color.Purple))
                    .ParseBlock<RemoveFooter>()
                    .ParseBlock<RemoveHeader>()
                    .Show(Color.Yellow);

            pipeline.Done();
        }
        
        public static void MergeBlockLines(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf").Page(1)
                    .Output($"bin/{basename}-tmp-output.pdf")
                    .ParsePdf<ProcessPdfText>()
                    .ParseBlock<GroupLines>()
                    .ParseBlock<FindInitialBlockset>()
                    //.ParseBlock<TestSplitBlocksets>()
                    //.Show(Color.Red)
                    .ShowLine(Color.Gray)
                    .ParseBlock<MergeBlockLines>()
                    .Show(Color.Green)
                    //.ParseBlock<BreakColumns>()
                    .Validate<RemoveFooter>().ShowErrors(p => p.Show(Color.Purple))
                    .Validate<RemoveHeader>().ShowErrors(p => p.Show(Color.Purple))
                    .ParseBlock<RemoveFooter>()
                    .ParseBlock<RemoveHeader>();
                    //.Show(Color.Yellow);

            pipeline.Done();
        }

        public static void RemoveTables(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/{basename}-table-output.pdf")
                    .Page(1)
                    .ParsePdf<PreProcessTables>()
                        .ParseBlock<IdentifyTables>()
                        .Show(Color.Green)
                    .ParsePdf<ProcessPdfText>()
                        .ParseBlock<RemoveTableText>()
                        .ParseBlock<GroupLines>()
                        .Show(Color.Red);
            
            pipeline.Done();
        }

        public static void OrderBlocksets(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/{basename}-table-output.pdf")
                    .Page(1)
                    .ParsePdf<PreProcessTables>()
                        .ParseBlock<IdentifyTables>()
                    .ParsePdf<ProcessPdfText>()
                        .ParseBlock<RemoveTableText>()
                        .ParseBlock<GroupLines>()
                        .ParseBlock<FindInitialBlockset>()
                        .ParseBlock<BreakColumns>()
                        .Validate<RemoveFooter>().ShowErrors(p => p.Show(Color.Purple))
                        .Validate<RemoveHeader>().ShowErrors(p => p.Show(Color.Purple))
                        .ParseBlock<RemoveFooter>()
                        .ParseBlock<RemoveHeader>()
                        .ParseBlock<OrderBlocksets>()
                        .Show(Color.Orange)
                        .ShowLine(Color.Black);

            pipeline.Done();
        }


        public static void TestEnumFiles(string foldername, Action<string> action)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.EnumFiles(foldername, action);
        }

        public static void RunCorePdf(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/{basename}-table-output.pdf")
                    .Page(1)
                    .ParsePdf<PreProcessTables>()
                        .ParseBlock<IdentifyTables>()
                        .Show(Color.Green)
                    .ParsePdf<ProcessPdfText>()
                        .ParseBlock<RemoveTableText>()
                        .ParseBlock<GroupLines>()
                        .Show(Color.LightGray)
                        .ParseBlock<FindInitialBlockset>()
                        .ParseBlock<BreakColumns>()
                        .Validate<RemoveFooter>().ShowErrors(p => p.Show(Color.Purple))
                        .Validate<RemoveHeader>().ShowErrors(p => p.Show(Color.Purple))
                        .ParseBlock<RemoveFooter>()
                        .ParseBlock<RemoveHeader>()
                        .ParseBlock<OrderBlocksets>()
                        .Show(Color.Orange)
                        .ShowLine(Color.Black);

            pipeline.Done();
        }

        public static IEnumerable<TextLine> GetLinesUsingPipeline(string basename)
        {
            var pipeline = new Execution.Pipeline();

            var textOutput =
            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/{basename}-tmp-output.pdf")
                    .Page(1)
                    .ParsePdf<PreProcessTables>()
                        .ParseBlock<IdentifyTables>()
                    .ParsePdf<ProcessPdfText>()
                        .ParseBlock<RemoveTableText>()
                        .ParseBlock<GroupLines>()
                        .ParseBlock<FindInitialBlockset>()
                        .ParseBlock<BreakColumns>()
                        .Validate<RemoveFooter>().ShowErrors(p => p.Show(Color.Purple))
                        .Validate<RemoveHeader>().ShowErrors(p => p.Show(Color.Purple))
                        .ParseBlock<RemoveFooter>()
                        .ParseBlock<RemoveHeader>()
                        .ParseBlock<OrderBlocksets>()
                    .Text<CreateStructures>();

            pipeline.Done();

            var lines = textOutput.CurrentText.AllText;

            return lines;
        }
        public static IEnumerable<TextLine> CenteredLines(string basename)
        {
            var pipeline = new Execution.Pipeline();

            var textOutput =
            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/{basename}-tmp-output.pdf")
                    .Page(1)
                    .ParsePdf<PreProcessTables>()
                        .ParseBlock<IdentifyTables>()
                    .ParsePdf<ProcessPdfText>()
                        .ParseBlock<RemoveTableText>()
                        .ParseBlock<GroupLines>()
                        .ParseBlock<FindInitialBlockset>()
                        .ParseBlock<BreakColumns>()
                        .Validate<RemoveFooter>().ShowErrors(p => p.Show(Color.Purple))
                        .Validate<RemoveHeader>().ShowErrors(p => p.Show(Color.Purple))
                        .ParseBlock<RemoveFooter>()
                        .ParseBlock<RemoveHeader>()
                        .ParseBlock<OrderBlocksets>()
                        .Show(Color.Blue)
                    .Text<CreateStructures>()
                        .ParseText<CenteredLines>()
                        .Show(Color.Orange)
                        ;

            pipeline.Done();

            var lines = textOutput.CurrentText.AllText;

            return lines;
        }
        public static void TestPipeline(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf") //.Output
                    .Page(1)  // .AllPages( p => p.CurrentPage )
                    .ParsePdf<PreProcessTables>()
                        .Output($"bin/{basename}-table-output.pdf")
                        .DebugBreak(c => c != null)
                        .Show(Color.Green)
                    .Show<TableCell>(b => b.Op == 1, Color.Green)
                    .Output("lines")
                    .ParsePdf<ProcessPdfText>()
                    //.ParseBlock<PreProcessTables>() 
                    .ParseBlock<FindInitialBlockset>()
                    .ParseBlock<BreakColumns>()
                        .Validate<BreakColumns>(Color.Red)
                    .ParseBlock<RemoveHeader>().Debug(Color.Blue)
                    .ParseBlock<RemoveFooter>().Debug(Color.Blue)
                        .Validate<ValidFooter>(p => new Exception())
                    //.ParseBlock<CreateLines>() -- GroupLines
                    .Text<CreateStructures>()
                        // .Show(Color.Yellow)
                    .ParseText<ProcessParagraphs>()
                    //.ParseText<ProcessStructure>()
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
