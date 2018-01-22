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
using System.IO;
using System.Text;

namespace PdfTextReader
{
    class ExamplesWork
    {
        static string pdfsDir;
        static string _inputFolder;

        public static void PrintAllSteps(string name, string inputfolder, string outputfolder)
        {
            pdfsDir = Directory.CreateDirectory($"{outputfolder}/PDFs-Steps").FullName;
            _inputFolder = inputfolder;

            ExamplesWork.Blocks(name);
            ExamplesWork.BlockLines(name);
            ExamplesWork.BlockSets(name);
            ExamplesWork.FollowLine(name);
            ExamplesWork.BreakColumn(name);
            ExamplesWork.Tables(name);
            ExamplesWork.FindTables(name);
            ExamplesWork.BadOrder(name);
            ExamplesWork.CorrectOrder(name);
            ExamplesWork.FindIds(name);
        }

        public static void Blocks(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"{_inputFolder}/{basename}.pdf")
                    .Output($"{pdfsDir}/01-blocks-{basename}-output.pdf")
                    .Page(1)
                    .ParsePdf<ProcessPdfText>()
                    .Show(Color.Orange);

            pipeline.Done();
        }

        public static void BlockLines(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"{_inputFolder}/{basename}.pdf")
                    .Output($"{pdfsDir}/02-blockline-{basename}-output.pdf")
                    .Page(1)
                    .ParsePdf<ProcessPdfText>()
                        .ParseBlock<GroupLines>()
                    .Show(Color.Red);

            pipeline.Done();
        }
        public static void BlockSets(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"{_inputFolder}/{basename}.pdf")
                    .Output($"{pdfsDir}/03-blocksets-{basename}-output.pdf")
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

            pipeline.Input($"{_inputFolder}/{basename}.pdf")
                    .Output($"{pdfsDir}/04-followline-{basename}-output.pdf")
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

            pipeline.Input($"{_inputFolder}/{basename}.pdf")
                    .Output($"{pdfsDir}/05-breakcolumn-{basename}-output.pdf")
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

            pipeline.Input($"{_inputFolder}/{basename}.pdf")
                    .Output($"{pdfsDir}/06-tables-{basename}-output.pdf")
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

            pipeline.Input($"{_inputFolder}/{basename}.pdf")
                    .Output($"{pdfsDir}/07-findtables-{basename}-output.pdf")
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

            pipeline.Input($"{_inputFolder}/{basename}.pdf")
                    .Output($"{pdfsDir}/08-badorders-{basename}-output.pdf")
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

            pipeline.Input($"{_inputFolder}/{basename}.pdf")
                    .Output($"{pdfsDir}/09-orders-{basename}-output.pdf")
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

            pipeline.Input($"{_inputFolder}/{basename}.pdf")
                    .Output($"{pdfsDir}/10-ids-{basename}-output.pdf")
                    .AllPages(page =>
                    {
                        page.ParsePdf<ProcessPdfText>()
                            .Validate<RemoveSmallFonts>().ShowErrors(p => p.ShowText(Color.Green));
                    });

            pipeline.Done();
        }
        
        public static void ParseFinal(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"{_inputFolder}/{basename}.pdf")
                    .Output($"{pdfsDir}/11-final-{basename}-output.pdf")
                    .AllPages(page =>
                    {
                        page.ParsePdf<PreProcessTables>()
                                .ParseBlock<IdentifyTables>()
                            .ParsePdf<PreProcessImages>()
                                .ParseBlock<RemoveOverlapedImages>()
                            .ParsePdf<ProcessPdfText>()
                                    .Validate<RemoveSmallFonts>().ShowErrors(p => p.ShowText(Color.Green))
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
                                .ParseBlock<OrderBlocksets>()
                                    .Show(Color.Orange)
                                .ShowLine(Color.Black);
                    });                    

            pipeline.Done();
        }

    }
}
