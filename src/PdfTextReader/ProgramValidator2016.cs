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
using System.IO;

namespace PdfTextReader
{
    public class ProgramValidator2016
    {
        static string logDir;
        static string xmlDir;

        public static void Process(string basename, string inputfolder, string outputfolder)
        {
            logDir = Directory.CreateDirectory($"{outputfolder}/Log").FullName;
            xmlDir = Directory.CreateDirectory($"{outputfolder}/XMLs").FullName;

            PdfReaderException.ContinueOnException();

            ExamplesWork.PrintAllSteps(basename, inputfolder, outputfolder);

            var conteudos = GetTextLines(basename, inputfolder, outputfolder, out Execution.Pipeline pipeline)
                                .Log<AnalyzeLines>($"{logDir}/{basename}-lines.txt")
                            .ConvertText<CreateStructures, TextStructure>()
                                .Log<AnalyzeStructuresCentral>($"{logDir}/{basename}-central.txt")
                            .PrintAnalytics($"{logDir}/{basename}-print-analytics.txt")
                            .ConvertText<CreateTextSegments, TextSegment>()
                                .Log<AnalyzeSegmentTitles>($"{logDir}/{basename}-tree.txt")
                                .Log<AnalyzeSegmentStats>($"{logDir}/{basename}-segments-stats.txt")
                                .Log<AnalyzeSegments2>($"{logDir}/{basename}-segments.csv")
                            .ConvertText<CreateTreeSegments, TextSegment>()
                            .ConvertText<TransformConteudo, Conteudo>()
                            .ConvertText<AggregateAnexo, Conteudo>()
                            .ToList();
            //Create XML
            var createArticle = new TransformArtigo2();
            var artigos = createArticle.Create(conteudos);
            createArticle.CreateXML(artigos, xmlDir, basename);

            pipeline.Done();

            var validator = new ProgramValidatorXML();
            validator.ValidateArticle(outputfolder);
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
                                  .ParseBlock<ReplaceCharacters>()
                                  .ParseBlock<GroupLines>()
                                      .ParseBlock<RemoveTableDotChar>()
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
