using PdfTextReader.Base;
using PdfTextReader.Execution;
using PdfTextReader.ExecutionStats;
using PdfTextReader.Parser;
using PdfTextReader.PDFCore;
using PdfTextReader.PDFText;
using PdfTextReader.TextStructures;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PdfTextReader
{
    class ExamplesWork
    {
        public static void Blocks(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/work/work-01-blocks-{basename}-output.pdf")
                    .Page(1)
                    .ParsePdf<ProcessPdfText>()
                    .Show(Color.Orange);

            pipeline.Done();
        }

        public static void BlockLines(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/work/work-02-blockline-{basename}-output.pdf")
                    .Page(1)
                    .ParsePdf<ProcessPdfText>()
                        .ParseBlock<GroupLines>()
                    .Show(Color.Red);

            pipeline.Done();
        }
        public static void BlockSets(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/work/work-03-blocksets-{basename}-output.pdf")
                    .Page(1)
                    .ParsePdf<ProcessPdfText>()
                        .ParseBlock<GroupLines>()
                        .ParseBlock<FindInitialBlockset>()
                    .Show(Color.Orange);

            pipeline.Done();
        }

        public static void FollowLine(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/work/work-04-followline-{basename}-output.pdf")
                    .Page(1)
                    .ParsePdf<ProcessPdfText>()
                        .ParseBlock<GroupLines>()
                            .ShowLine(Color.Green)
                        .ParseBlock<FindInitialBlockset>()
                            .Show(Color.Orange);

            pipeline.Done();
        }
        
        public static void BreakColumn(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/work/work-05-breakcolumn-{basename}-output.pdf")
                    .Page(1)
                    .ParsePdf<PreProcessTables>()
                                .ParseBlock<IdentifyTables>()
                            .ParsePdf<PreProcessImages>()
                                .ParseBlock<RemoveOverlapedImages>()
                            .ParsePdf<ProcessPdfText>()
                                .ParseBlock<GroupLines>()
                                    .Validate<RemoveHeaderImage>().ShowErrors(p => p.Show(Color.Purple))
                                .ParseBlock<RemoveHeaderImage>()
                                .ParseBlock<FindInitialBlocksetWithRewind>()
                                .ParseBlock<BreakColumnsLight>()
                            .Show(Color.Orange);

            pipeline.Done();
        }

        public static void Tables(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/work/work-06-tables-{basename}-output.pdf")
                    .Page(1)
                    .ParsePdf<PreProcessTables>()
                                .ParseBlock<IdentifyTables>()
                                .Show(Color.Red)
                            .ParsePdf<PreProcessImages>()
                                .ParseBlock<RemoveOverlapedImages>()
                            .ParsePdf<ProcessPdfText>()
                                .ParseBlock<GroupLines>()
                                    .Validate<RemoveHeaderImage>().ShowErrors(p => p.Show(Color.Purple))
                                .ParseBlock<RemoveHeaderImage>()
                                .ParseBlock<FindInitialBlocksetWithRewind>()
                                .ParseBlock<BreakColumnsLight>()
                                .ParseBlock<AddTableSpace>()
                            .Show(Color.Orange)
                            .ShowLine(Color.Black);

            pipeline.Done();
        }

        public static void FindTables(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/work/work-07-findtables-{basename}-output.pdf")
                    .Page(1)
                    .ParsePdf<PreProcessTables>()
                                .ParseBlock<IdentifyTables>()
                            .ParsePdf<PreProcessImages>()
                                    .Validate<RemoveOverlapedImages>().ShowErrors(p => p.Show(Color.Red))
                                .ParseBlock<RemoveOverlapedImages>()
                            .ParsePdf<ProcessPdfText>()
                                .ParseBlock<RemoveSmallFonts>()
                                .ParseBlock<MergeTableText>()
                                .ParseBlock<HighlightTextTable>()
                                .ParseBlock<RemoveTableText>()
                                .ParseBlock<GroupLines>()
                                    .Validate<RemoveHeaderImage>().ShowErrors(p => p.Show(Color.Purple))
                                .ParseBlock<RemoveHeaderImage>()
                                .ParseBlock<FindInitialBlocksetWithRewind>()
                                .ParseBlock<BreakColumnsLight>()
                                .ParseBlock<AddTableSpace>()
                                .ParseBlock<BreakInlineElements>()
                                    .Show(Color.Red);
            
            pipeline.Done();
        }
        
        public static void BadOrder(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/work/work-08-badorders-{basename}-output.pdf")
                    .Page(1)
                    .ParsePdf<PreProcessTables>()
                                .ParseBlock<IdentifyTables>()
                            .ParsePdf<PreProcessImages>()
                                .ParseBlock<RemoveOverlapedImages>()
                            .ParsePdf<ProcessPdfText>()
                                .ParseBlock<RemoveSmallFonts>()
                                .ParseBlock<MergeTableText>()
                                .ParseBlock<HighlightTextTable>()
                                .ParseBlock<RemoveTableText>()
                                .ParseBlock<GroupLines>()
                                    .Validate<RemoveHeaderImage>().ShowErrors(p => p.Show(Color.Purple))
                                .ParseBlock<RemoveHeaderImage>()
                                .ParseBlock<FindInitialBlocksetWithRewind>()
                                .ParseBlock<BreakColumnsLight>()
                                    .Validate<RemoveFooter>().ShowErrors(p => p.Show(Color.Purple))
                                    .ParseBlock<RemoveFooter>()
                                .ParseBlock<AddTableSpace>()
                                .ParseBlock<AddImageSpace>()
                                .ParseBlock<BreakInlineElements>()
                                .ParseBlock<ResizeBlocksets>()
                                    .Validate<ResizeBlocksets>().ShowErrors(p => p.Show(Color.Red))
                                .Show(Color.Orange)
                                .ShowLine(Color.Black);

            pipeline.Done();
        }

        public static void CorrectOrder(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/work/work-09-orders-{basename}-output.pdf")
                    .Page(1)
                    .ParsePdf<PreProcessTables>()
                                .ParseBlock<IdentifyTables>()
                            .ParsePdf<PreProcessImages>()
                                .ParseBlock<RemoveOverlapedImages>()
                            .ParsePdf<ProcessPdfText>()
                                .ParseBlock<RemoveSmallFonts>()
                                .ParseBlock<MergeTableText>()
                                .ParseBlock<HighlightTextTable>()
                                .ParseBlock<RemoveTableText>()
                                .ParseBlock<GroupLines>()
                                    .Validate<RemoveHeaderImage>().ShowErrors(p => p.Show(Color.Purple))
                                .ParseBlock<RemoveHeaderImage>()
                                .ParseBlock<FindInitialBlocksetWithRewind>()
                                .ParseBlock<BreakColumnsLight>()
                                    .Validate<RemoveFooter>().ShowErrors(p => p.Show(Color.Purple))
                                    .ParseBlock<RemoveFooter>()
                                .ParseBlock<AddTableSpace>()
                                .ParseBlock<AddImageSpace>()
                                .ParseBlock<BreakInlineElements>()
                                .ParseBlock<ResizeBlocksets>()
                                    .Validate<ResizeBlocksets>().ShowErrors(p => p.Show(Color.Red))
                                .ParseBlock<OrderBlocksets>()
                                .Show(Color.Orange)
                                .ShowLine(Color.Black);

            pipeline.Done();
        }
        public static void FindIds(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/work/work-10-ids-{basename}-output.pdf")
                    .AllPages(page =>
                    {
                        page.ParsePdf<ProcessPdfText>()
                            .Validate<RemoveSmallFonts>().ShowErrors(p => p.ShowText(Color.Green));
                    });

            pipeline.Done();
        }
    }
}
