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
using System.Linq;

namespace PdfTextReader
{
    public class ProgramValidator2010
    {
        public static void Process(string basename, string inputfolder, string outputfolder)
        {
            PdfReaderException.ContinueOnException();

            var artigos = GetTextLines(basename, inputfolder, outputfolder, out Execution.Pipeline pipeline)
                            .ConvertText<CreateTextLineIndex, TextLine>()
                            .ConvertText<CreateStructures, TextStructure>()
                            .ConvertText<CreateTextSegments, TextSegment>()
                            .ConvertText<CreateTreeSegments, TextSegment>()
                                .Log<AnalyzeTreeStructure>($"{outputfolder}/{basename}-tree.txt")
                                .ToList();

            Console.WriteLine($"FILENAME: {pipeline.Filename}");

            var validation = pipeline.Statistics.Calculate<ValidateFooter, StatsPageFooter>();
            var layout = (ValidateLayout)pipeline.Statistics.Calculate<ValidateLayout, StatsPageLayout>();
            var overlap = (ValidateOverlap)pipeline.Statistics.Calculate<ValidateOverlap, StatsBlocksOverlapped>();

            var pagesLayout = layout.GetPageErrors().ToList();
            var pagesOverlap = overlap.GetPageErrors().ToList();
            var pages = pagesLayout.Concat(pagesOverlap).Distinct().OrderBy(t => t).ToList();

            if( pages.Count > 0 )
            {
                ExtractPages($"{outputfolder}/{basename}-parser", $"{outputfolder}/{basename}-parser-errors", pages);
            }

            pipeline.Done();
        }


        static PipelineText<TextLine> GetTextLines(string basename, string inputfolder, string outputfolder, out Execution.Pipeline pipeline)
        {
            pipeline = new Execution.Pipeline();


            var result =
            pipeline.Input($"{inputfolder}/{basename}.pdf")
                    .Output($"{outputfolder}/{basename}-parser.pdf")
                    .AllPagesExcept<CreateTextLines>(new int[] { }, page =>
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
                                  .ParseBlock<OrganizePageLayout>()
                                  .ParseBlock<CheckOverlap>()
                                  .Validate<ValidatePositiveCoordinates>().ShowErrors(p => p.Show(Color.Red))
                    );

            return result;
        }
        
        static void ExtractPages(string basename, string outputname, IList<int> pages)
        {
            var pipeline = new Execution.Pipeline();

            pipeline.Input($"{basename}.pdf")
                    .ExtractPages($"{outputname}.pdf", pages);
        }
    }
}
