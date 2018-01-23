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
    public class ProgramValidator2010
    {
        public static void Process(string basename, string inputfolder, string outputfolder)
        {
            //PdfReaderException.DisableWarnings();
            PdfReaderException.ContinueOnException();

            var artigos = GetTextLines(basename, inputfolder, outputfolder, out Execution.Pipeline pipeline)
                                .ConvertText<CreateTextLineIndex, TextLine>()
                                .ConvertText<CreateStructures, TextStructure>()
                                .ConvertText<CreateTextSegments, TextSegment>()
                                .ConvertText<CreateTreeSegments, TextSegment>()
                                .Log<AnalyzeTreeStructure>(Console.Out)
                                .ToList();

            //var validation = pipeline.Statistics.Calculate<ValidateFooter, StatsPageFooter>();

            pipeline.Done();
        }


        static PipelineText<TextLine> GetTextLines(string basename, string inputfolder, string outputfolder, out Execution.Pipeline pipeline)
        {
            pipeline = new Execution.Pipeline();


            var result =
            pipeline.Input($"{inputfolder}/{basename}.pdf")
                    .Output($"{outputfolder}/{basename}.pdf")
                    .AllPagesExcept<CreateTextLines>(new int[] { }, page =>
                              page.ParsePdf<PreProcessTables>()
                                  .ParseBlock<IdentifyTables>()
                                  //.Show(Color.Blue)
                              .ParsePdf<PreProcessImages>()
                                  //.Validate<RemoveOverlapedImages>().ShowErrors(p => p.Show(Color.Blue))
                                  .ParseBlock<RemoveOverlapedImages>()
                              .ParsePdf<ProcessPdfText>()
                                  //.Validate<RemoveSmallFonts>().ShowErrors(p => p.ShowText(Color.Green))
                                  .ParseBlock<RemoveSmallFonts>()
                                  //.Validate<MergeTableText>().ShowErrors(p => p.Show(Color.Blue))
                                  .ParseBlock<MergeTableText>()
                                  .Validate<HighlightTextTable>().ShowErrors(p => p.Show(Color.Blue))
                                  .Validate<HighlightTextTable>().ShowErrors(p => p.Show(Color.Red))
                                  .Validate<HighlightTextTable>().ShowErrors(p => p.Show(Color.Purple))
                                  .ParseBlock<HighlightTextTable>()
                                  .ParseBlock<RemoveTableText>()
                                  .ParseBlock<ReplaceCharacters>()
                                  .ParseBlock<GroupLines>()
                                  .ParseBlock<RemoveTableDotChar>()
                                      .Show(Color.Yellow)
                                      .Validate<RemoveHeaderImage>().ShowErrors(p => p.Show(Color.Green))
                                  .ParseBlock<RemoveHeaderImage>()
                                  .ParseBlock<FindInitialBlocksetWithRewind>()
                                      .Show(Color.Gray)
                                  .ParseBlock<BreakColumnsLight>()
                                  //.ParseBlock<BreakColumns>()
                                  .ParseBlock<AddTableSpace>()
                                  .ParseBlock<RemoveTableOverImage>()
                                  .ParseBlock<RemoveImageTexts>()
                                  .ParseBlock<AddImageSpace>()
                                      .Validate<RemoveFooter>().ShowErrors(p => p.Show(Color.Purple))
                                      .ParseBlock<RemoveFooter>()
                                  .ParseBlock<BreakInlineElements>()
                                  .ParseBlock<ResizeBlocksets>()
                                      .Validate<ResizeBlocksets>().ShowErrors(p => p.Show(Color.Gray))
                                  .ParseBlock<OrderBlocksets>()
                                  .Show(Color.Orange)
                                  .ShowLine(Color.Black)
                                  //.Validate<ValidatePositiveCoordinates>().ShowErrors(p => p.Show(Color.Red))
                    );

            return result;
        }        
    }
}
