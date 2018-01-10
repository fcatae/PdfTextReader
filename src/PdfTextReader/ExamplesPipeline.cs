using PdfTextReader.Base;
using PdfTextReader.Execution;
using PdfTextReader.Parser;
using PdfTextReader.PDFCore;
using PdfTextReader.PDFText;
using PdfTextReader.TextStructures;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace PdfTextReader
{
    class ExamplesPipeline
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

        public static void UseTextBackground(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/{basename}-table-output.pdf")
                    .Page(1)
                    .ParsePdf<PreProcessTables>()
                        .ParseBlock<IdentifyTables>()
                        .Show(Color.Green)
                    .ParsePdf<ProcessPdfText>()
                        //.ParseBlock<RemoveTableText>()
                        .ParseBlock<HighlightTextTable>()
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

        public static void OrderBlocksetsWithTables(string basename)
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
                        .ParseBlock<AddTableSpace>()
                        .ParseBlock<OrderBlocksets>()
                        .Show(Color.Orange)
                        .ShowLine(Color.Black);

            pipeline.Done();
        }

        public static void OrderBlocksetsWithTablesDebug(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/{basename}-table-output.pdf")
                    .Page(1)
                    .ParsePdf<PreProcessTables>()
                        .ParseBlock<IdentifyTables>()
                    .ParsePdf<PreProcessImages>()
                    .ParsePdf<ProcessPdfText>()
                        .ParseBlock<RemoveTableText>()
                        .ParseBlock<GroupLines>()
                            .Validate<RemoveHeader>().ShowErrors(p => p.Show(Color.Purple))
                            .ParseBlock<RemoveHeaderImage>()
                        .ParseBlock<FindInitialBlockset>()
                        .ParseBlock<BreakColumns>()
                        .Show(Color.LightGray)
                        //.Validate<RemoveFooter>().ShowErrors(p => p.Show(Color.Purple))
                        //.ParseBlock<RemoveFooter>()
                        //.ParseBlock<AddTableSpace>()
                        //.ParseBlock<OrderBlocksets>()
                        .Show(Color.Orange)
                        .ShowLine(Color.Black);

            pipeline.Done();
        }

        public static void ResizeBlocksets(string basename)
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
                        .ParseBlock<AddTableSpace>()
                        .ParseBlock<ResizeBlocksets>()
                        .ParseBlock<OrderBlocksets>()
                        .Show(Color.Orange)
                        .ShowLine(Color.Black);

            pipeline.Done();
        }

        public static void ProcessImages(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/{basename}-img-output.pdf")
                    .Page(1)
                    .ParsePdf<PreProcessImages>()
                        .Show(Color.Red);

            pipeline.Done();
        }

        public static void RemoveImageTexts(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/{basename}-img-output.pdf")
                    .Page(1)
                    .ParsePdf<PreProcessImages>()
                    .ParsePdf<ProcessPdfText>()
                        .ParseBlock<RemoveImageTexts>()
                        .Show(Color.Red);

            pipeline.Done();
        }

        public static void TestEnumFiles(string foldername, Action<string> action)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.EnumFiles(foldername, action);
        }
        
        public static void AddImageSpace(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/{basename}-img-output.pdf")
                    .Page(1)
                    .ParsePdf<PreProcessTables>()
                        .ParseBlock<IdentifyTables>()
                    .ParsePdf<PreProcessImages>()
                    .ParsePdf<ProcessPdfText>()
                        .ParseBlock<RemoveTableText>()
                        .ParseBlock<GroupLines>()
                        .ParseBlock<FindInitialBlockset>()
                        .ParseBlock<AddTableSpace>()
                        .ParseBlock<AddImageSpace>()
                        .Show(Color.Orange);

            pipeline.Done();
        }

        public static void BreakInlineElements(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/{basename}-tmp-output.pdf")
                    .Page(1)
                    .ParsePdf<PreProcessTables>()
                        .ParseBlock<IdentifyTables>()
                    .ParsePdf<PreProcessImages>()
                    .ParsePdf<ProcessPdfText>()
                        .ParseBlock<RemoveTableText>()
                        .ParseBlock<GroupLines>()
                        .ParseBlock<FindInitialBlockset>()
                        .ParseBlock<BreakColumns>()
                        .Validate<RemoveFooter>().ShowErrors(p => p.Show(Color.Purple))
                        .Validate<RemoveHeaderImage>().ShowErrors(p => p.Show(Color.Purple))
                        .ParseBlock<RemoveFooter>()
                        .ParseBlock<RemoveHeaderImage>()
                        .ParseBlock<AddTableSpace>()
                        .ParseBlock<AddImageSpace>()
                        .Validate<BreakInlineElements>().ShowErrors(p => p.Show(Color.Green))
                        .ParseBlock<BreakInlineElements>()
                        .Show(Color.Orange);
                        //.ParseBlock<ResizeBlocksets>()
                        //.ParseBlock<OrderBlocksets>()
                        //.Show(Color.Orange);                        

            pipeline.Done();
        }

        public static void RemoveOverlapedImages(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/{basename}-tmp-output.pdf")
                    .Page(1)
                    //.ParsePdf<PreProcessTables>()
                    //    .ParseBlock<IdentifyTables>()
                    .ParsePdf<PreProcessImages>()
                        .Validate<RemoveOverlapedImages>().ShowErrors(p => p.Show(Color.Red))
                        .ParseBlock<RemoveOverlapedImages>()
                        .Show(Color.Green);
            pipeline.Done();
        }

        public static void OverlappedTables(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/{basename}-table-output.pdf")
                    .Page(1)
                    .ParsePdf<PreProcessTables>()
                        .ParseBlock<IdentifyTables>()
                    .ParsePdf<PreProcessImages>()
                        .Validate<RemoveOverlapedImages>().ShowErrors(p => p.Show(Color.Red))
                        .ParseBlock<RemoveOverlapedImages>()
                    .ParsePdf<ProcessPdfText>()
                        .ParseBlock<RemoveTableText>()
                        .ParseBlock<GroupLines>()
                        .ParseBlock<FindInitialBlockset>()
                        .ParseBlock<BreakColumns>()
                        .Validate<RemoveFooter>().ShowErrors(p => p.Show(Color.Purple))
                        .Validate<RemoveHeaderImage>().ShowErrors(p => p.Show(Color.Purple))
                        .ParseBlock<RemoveFooter>()
                        .ParseBlock<RemoveHeaderImage>()
                        .ParseBlock<AddTableSpace>()
                        .ParseBlock<AddImageSpace>()
                        //.ParseBlock<BreakInlineElements>()
                        //.ParseBlock<ResizeBlocksets>()
                        //.ParseBlock<OrderBlocksets>()
                        .Show(Color.Orange)
                        .ShowLine(Color.Black);

            pipeline.Done();
        }

        public static void ValidateResizeBlock(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/{basename}-test-output.pdf")
                    .Page(1)
                    .ParsePdf<PreProcessTables>()
                        .ParseBlock<IdentifyTables>()
                    .ParsePdf<PreProcessImages>()
                        .Validate<RemoveOverlapedImages>().ShowErrors(p => p.Show(Color.Red))
                        .ParseBlock<RemoveOverlapedImages>()
                    .ParsePdf<ProcessPdfText>()
                        .ParseBlock<RemoveTableText>()
                        .ParseBlock<GroupLines>()
                        .ParseBlock<FindInitialBlockset>()
                        .ParseBlock<BreakColumns>()
                        //.Validate<RemoveFooter>().ShowErrors(p => p.Show(Color.Purple))
                        //.Validate<RemoveHeaderImage>().ShowErrors(p => p.Show(Color.Purple))
                        .ParseBlock<RemoveFooter>()
                        .ParseBlock<RemoveHeaderImage>()
                        .ParseBlock<AddTableSpace>()
                        .ParseBlock<AddImageSpace>()
                        //.ParseBlock<BreakInlineElements>()
                        .Validate<ResizeBlocksets>().ShowErrors(p => p.Show(Color.Red));

            pipeline.Done();
        }

        public static void DetectInvisibleTable(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/{basename}-table-output.pdf")
                    .Page(1)
                    .ParsePdf<PreProcessTables>()
                        .ParseBlock<IdentifyTables>()
                    .ParsePdf<PreProcessImages>()
                            .Validate<RemoveOverlapedImages>().ShowErrors(p => p.Show(Color.Red))
                        .ParseBlock<RemoveOverlapedImages>()
                    .ParsePdf<ProcessPdfText>()
                        .ParseBlock<RemoveTableText>()
                        .ParseBlock<GroupLines>()
                        .ParseBlock<FindInitialBlockset>()
                        .Show(Color.Yellow)
                        .ParseBlock<DetectInvisibleTable>()
                        .Show(Color.Green)
                        .Validate<DetectInvisibleTable>().ShowErrors(p => p.Show(Color.Red))
                        .ShowLine(Color.Black);

            pipeline.Done();
        }

        public static void RunCorePdf(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/{basename}-test-output.pdf")
                    .Page(1)
                    .ParsePdf<PreProcessTables>()
                        .ParseBlock<IdentifyTables>()
                    .ParsePdf<PreProcessImages>()
                            .Validate<RemoveOverlapedImages>().ShowErrors(p => p.Show(Color.Red))
                        .ParseBlock<RemoveOverlapedImages>()
                    .ParsePdf<ProcessPdfText>()
                        //.Validate<MergeTableText>().ShowErrors(p => p.Show(Color.Blue))
                        .ParseBlock<MergeTableText>()
                        //.Validate<HighlightTextTable>().ShowErrors(p => p.Show(Color.Green))
                        .ParseBlock<HighlightTextTable>()
                        .ParseBlock<RemoveTableText>()
                        .ParseBlock<GroupLines>()
                            .Validate<RemoveHeaderImage>().ShowErrors(p => p.Show(Color.Purple))
                            .ParseBlock<RemoveHeaderImage>()
                        .ParseBlock<FindInitialBlocksetWithRewind>()
                        .ParseBlock<BreakColumns>()
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
        
        public static void RemoveHeaderImage(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/{basename}-img-output.pdf")
                    .Page(1)
                    .ParsePdf<PreProcessTables>()
                        .ParseBlock<IdentifyTables>()
                    .ParsePdf<PreProcessImages>()
                    .ParsePdf<ProcessPdfText>()
                        .ParseBlock<RemoveTableText>()
                        .ParseBlock<GroupLines>()
                        .ParseBlock<FindInitialBlockset>()
                        .Validate<RemoveFooter>().ShowErrors(p => p.Show(Color.Orange))
                        //.Validate<RemoveHeader>().ShowErrors(p => p.Show(Color.Orange))
                        .Validate<RemoveHeaderImage>().ShowErrors(p => p.Show(Color.Orange))
                        .ParseBlock<RemoveFooter>()
                        //.ParseBlock<RemoveHeader>()
                        .ParseBlock<RemoveHeaderImage>()
                        .Show(Color.Yellow);

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

        //public static IEnumerable<TextLine> CenteredLines(string basename)
        //{
        //    var pipeline = new Execution.Pipeline();

        //    var textOutput =
        //    pipeline.Input($"bin/{basename}.pdf")
        //            .Output($"bin/{basename}-tmp-output.pdf")
        //            .Page(1)
        //            .ParsePdf<PreProcessTables>()
        //                .ParseBlock<IdentifyTables>()
        //            .ParsePdf<ProcessPdfText>()
        //                .ParseBlock<RemoveTableText>()
        //                .ParseBlock<GroupLines>()
        //                .ParseBlock<FindInitialBlockset>()
        //                .ParseBlock<BreakColumns>()
        //                .Validate<RemoveFooter>().ShowErrors(p => p.Show(Color.Purple))
        //                .Validate<RemoveHeader>().ShowErrors(p => p.Show(Color.Purple))
        //                .ParseBlock<RemoveFooter>()
        //                .ParseBlock<RemoveHeader>()
        //                .ParseBlock<OrderBlocksets>()
        //                .Show(Color.Blue)
        //            .Text<CreateStructures>()
        //                .ConvertText<CenteredLines, TextStructure>()
        //                .Show(Color.Orange)
        //                ;

        //    pipeline.Done();
            
        //    return null;
        //}


        public static IEnumerable<Artigo> CreateArtigos(string basename)
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
                        .ConvertText<CreateParagraphs, TextStructure>()
                        .ConvertText<TransformArtigo, Artigo>()
                        //.Show(Color.Orange)
                        ;
            
            var artigos = pipeline.GetResults<Artigo>();

            var procParser = new ProcessParser();
            procParser.XMLWriter(artigos, $"bin/{basename}");

            pipeline.Done();

            return null;
        }

        public static void Extract(string basename, int pages)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf")
                    .Extract($"bin/{basename}-table-output.pdf", 1, pages);
        }

        public static void ExtractPage(string basename, int page)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf")
                    .Extract($"bin/{basename}-p{page}.pdf", page, page);
        }

        public static void Multipage(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/{basename}-page-output.pdf")
                    .AllPages(p =>
                    {
                        p.ParsePdf<ProcessPdfText>()
                            .ParseBlock<GroupLines>()
                            .Show(Color.Orange);
                    });                    

            pipeline.Done();
        }

        public static void MultipageCore(string basename)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/{basename}-page-output.pdf")
                    .AllPages(p => ProcessPage(p));

            pipeline.Done();
        }
        
        public static IEnumerable<Artigo> MultipageCreateArtigos(string basename)
        {
            var pipeline = new Execution.Pipeline();

            var result = 
            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/{basename}-page-output.pdf")
                    .AllPages<CreateStructures>(p =>
                    {
                        ProcessPage(p);
                        //p.ParsePdf<PreProcessTables>()
                        //        .ParseBlock<IdentifyTables>()
                        //    .ParsePdf<ProcessPdfText>()
                        //        .ParseBlock<RemoveTableText>()
                        //        .ParseBlock<GroupLines>()
                        //        .ParseBlock<FindInitialBlockset>();
                    })
                    .ConvertText<CreateParagraphs, TextStructure>()
                        .DebugCount("Paragraphs")
                        .DebugPrint("Paragraphs")
                    .ConvertText<TransformArtigo, Artigo>()
                        .DebugCount("Artigo")
                        .DebugPrint("Artigo")
                    .ToEnumerable()
                    .ToArray();

            pipeline.Done();

            var procParser = new ProcessParser();
            procParser.XMLWriter(result, $"bin/{basename}");
            
            return result;
        }

        public static PipelineText<TextStructure> GetTextStructures(string basename)
        {
            var pipeline = new Execution.Pipeline();

            var result =
            pipeline.Input($"bin/{basename}.pdf")
                    .Output($"bin/{basename}-page-output.pdf")
                    .AllPages<CreateStructures>( ProcessPage )
                    .ConvertText<CreateParagraphs, TextStructure>();

            return result;
        }
        public static void ProcessPage(PipelineInputPdf.PipelineInputPdfPage page)
        {
            page.ParsePdf<PreProcessTables>()
                        .ParseBlock<IdentifyTables>()
                    .ParsePdf<PreProcessImages>()
                            .Validate<RemoveOverlapedImages>().ShowErrors(p => p.Show(Color.Red))
                        .ParseBlock<RemoveOverlapedImages>()
                    .ParsePdf<ProcessPdfText>()
                        //.Validate<MergeTableText>().ShowErrors(p => p.Show(Color.Blue))
                        .ParseBlock<MergeTableText>()
                        //.Validate<HighlightTextTable>().ShowErrors(p => p.Show(Color.Green))
                        .ParseBlock<HighlightTextTable>()
                        .ParseBlock<RemoveTableText>()
                        .ParseBlock<GroupLines>()  
                            .Show(Color.Orange)
                            .Validate<RemoveHeaderImage>().ShowErrors(p => p.Show(Color.Purple))
                            .ParseBlock<RemoveHeaderImage>()
                        .ParseBlock<FindInitialBlocksetWithRewind>()
                        .ParseBlock<BreakColumns>()
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
        }        
    }
}
