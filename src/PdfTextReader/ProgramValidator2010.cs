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
            BasicFirstPageStats.Reset();
            PdfReaderException.ContinueOnException();

            var artigos = GetTextLines(basename, inputfolder, outputfolder, out Execution.Pipeline pipeline)
                            .ConvertText<CreateTextLineIndex, TextLine>()
                            .ConvertText<CreateStructures, TextStructure>()
                                .ShowPdf<ShowStructureCentral>($"{outputfolder}/{basename}-show-central.pdf")
                            .ConvertText<CreateTextSegments, TextSegment>()
                            .ConvertText<CreateTreeSegments, TextSegment>()
                                .Log<AnalyzeTreeStructure>($"{outputfolder}/{basename}-tree.txt")
                                .ToList();

            Console.WriteLine($"FILENAME: {pipeline.Filename}");

            var validation = pipeline.Statistics.Calculate<ValidateFooter, StatsPageFooter>();
            var layout = (ValidateLayout)pipeline.Statistics.Calculate<ValidateLayout, StatsPageLayout>();
            var overlap = (ValidateOverlap)pipeline.Statistics.Calculate<ValidateOverlap, StatsBlocksOverlapped>();
            var unhandled = (ValidateUnhandledExceptions)pipeline.Statistics.Calculate<ValidateUnhandledExceptions, StatsExceptionHandled>();

            var pagesLayout = layout.GetPageErrors().ToList();
            var pagesOverlap = overlap.GetPageErrors().ToList();
            var pagesUnhandled = unhandled.GetPageErrors().ToList();

            var pages = pagesLayout
                            .Concat(pagesOverlap)
                            .Concat(pagesUnhandled)
                            .Distinct().OrderBy(t => t).ToList();

            if( pages.Count > 0 )
            {
                ExtractPages($"{outputfolder}/{basename}-parser", $"{outputfolder}/errors/{basename}-parser-errors", pages);
            }

            pipeline.Done();
        }

        static PipelineText<TextLine> GetTextLines(string basename, string inputfolder, string outputfolder, out Execution.Pipeline pipeline)
        {
            string inputfile = $"{inputfolder}/{basename}.pdf";
            string outputfile = $"{outputfolder}/{basename}-parser.pdf";

            return Examples.GetTextLines(inputfile, outputfile, out pipeline);            
        }
        
        static void ExtractPages(string basename, string outputname, IList<int> pages)
        {
            using (var pipeline = new Execution.Pipeline())
            {
                pipeline.Input($"{basename}.pdf")
                        .ExtractPages($"{outputname}.pdf", pages);
            }
        }
    }
}
