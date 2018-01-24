using System;
using System.Collections.Generic;
using System.Text;
using PdfTextReader.Base;
using PdfTextReader.Parser;
using PdfTextReader.ExecutionStats;
using PdfTextReader.TextStructures;
using PdfTextReader.Execution;
using PdfTextReader.PDFText;
using System.Drawing;
using PdfTextReader.PDFCore;

namespace PdfTextReader
{
    public class ProgramValidator2012
    {
        public static void Process(string basename, string inputfolder, string outputfolder)
        {
            PdfReaderException.ContinueOnException();

            var conteudos = GetTextLines(basename, inputfolder, outputfolder, out Execution.Pipeline pipeline)
                                .Log<AnalyzeLines>($"{outputfolder}/{basename}-lines.txt")
                                .Log<AnalyzeCheckLines>($"{outputfolder}/{basename}-check-lines.txt")                                
                            .ConvertText<CreateStructures, TextStructure>()
                                .Log<AnalyzeStructuresCentral>($"{outputfolder}/{basename}-central.txt")
                            //.PrintAnalytics($"bin/{basename}-print-analytics.txt")
                            .ConvertText<CreateTextSegments, TextSegment>()
                                .Log<AnalyzeSegmentTitles>($"{outputfolder}/{basename}-tree.txt")
                                .Log<AnalyzeSegmentStats>($"{outputfolder}/{basename}-segments-stats.txt")
                                .Log<AnalyzeSegments2>($"{outputfolder}/{basename}-segments.csv")
                            .ConvertText<CreateTreeSegments, TextSegment>()
                            .ConvertText<TransformConteudo, Conteudo>()
                            .ToList();
            
            //Create XML
            var createArticle = new TransformArtigo();
            var artigos = createArticle.Create(conteudos);
            createArticle.CreateXML(artigos, outputfolder, basename);
            
            pipeline.Done();
        }

        public static void Validate(string basename, string inputfolder, string outputfolder)
        {
            var validator = new ProgramValidatorXML();
            validator.ValidateArticle($"{inputfolder}");
        }   
        
        static PipelineText<TextLine> GetTextLines(string basename, string inputfolder, string outputfolder, out Execution.Pipeline pipeline)
        {
            pipeline = new Execution.Pipeline();

            var result =
            pipeline.Input($"{inputfolder}/{basename}.pdf")
                    .Output($"{outputfolder}/{basename}-output.pdf")
                    .AllPagesExcept<CreateTextLines>(new int[] { }, page =>
                              page.ParsePdf<PreProcessTables>()
                                  .ParseBlock<IdentifyTables>()
                              .ParsePdf<PreProcessImages>()
                                      .Validate<RemoveOverlapedImages>().ShowErrors(p => p.Show(Color.Red))
                                  .ParseBlock<RemoveOverlapedImages>()
                              .ParsePdf<ProcessPdfText>()
                                  .Validate<RemoveSmallFonts>().ShowErrors(p => p.ShowText(Color.Green))
                                  .ParseBlock<RemoveSmallFonts>()
                                  //.Validate<MergeTableText>().ShowErrors(p => p.Show(Color.Blue))
                                  .ParseBlock<MergeTableText>()
                                  //.Validate<HighlightTextTable>().ShowErrors(p => p.Show(Color.Green))
                                  .ParseBlock<HighlightTextTable>()
                                  .ParseBlock<RemoveTableText>()
                                  .ParseBlock<GroupLines>()
                                      .Show(Color.Yellow)
                                      .Validate<RemoveHeaderImage>().ShowErrors(p => p.Show(Color.Purple))
                                  .ParseBlock<RemoveHeaderImage>()
                                  .ParseBlock<FindInitialBlocksetWithRewind>()
                                      .Show(Color.Gray)
                                  .ParseBlock<BreakColumnsLight>()
                                      //.ParseBlock<BreakColumns>()
                                      .Validate<RemoveFooter>().ShowErrors(p => p.Show(Color.Purple))
                                      .ParseBlock<RemoveFooter>()
                                  .ParseBlock<AddTableSpace>()
                                  .ParseBlock<AddImageSpace>()
                                  .ParseBlock<BreakInlineElements>()
                                  .ParseBlock<ResizeBlocksets>()
                                      .Validate<ResizeBlocksets>().ShowErrors(p => p.Show(Color.Red))
                                  .ParseBlock<OrderBlocksets>()
                                  .Show(Color.Orange)
                                  .ShowLine(Color.Black)
                    );

            return result;
        }        
    }
}
