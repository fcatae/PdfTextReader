﻿using System;
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

            var pipeline = new Execution.Pipeline();

            var artigos = GetTextLines(pipeline, basename, inputfolder, outputfolder)
                            .ConvertText<CreateTextLineIndex, TextLine>()
                            .ConvertText<PreCreateStructures, TextLine2>()
                            .ConvertText<CreateStructures2, TextStructure>()
                            .ConvertText<PreCreateTextSegments, TextStructureAgg>()
                            .ConvertText<AggregateStructures, TextStructure>()
                            .ShowPdf<ShowStructureCentral>($"{outputfolder}/{basename}-show-central.pdf")
                            .ConvertText<CreateTextSegments, TextSegment>()
                            .ConvertText<CreateTreeSegments, TextSegment>()
                                .Log<AnalyzeTreeStructure>($"{outputfolder}/{basename}-tree.txt")
                                .ToList();

            pipeline.ExtractOutput<ShowParserWarnings>($"{outputfolder}/errors/{basename}-parser-errors.pdf");
            
            pipeline.Done();
        }

        static PipelineText<TextLine> GetTextLines(Execution.Pipeline pipeline, string basename, string inputfolder, string outputfolder)
        {
            string inputfile = $"{inputfolder}/{basename}.pdf";
            string outputfile = $"{outputfolder}/{basename}-parser.pdf";

            return Examples.GetTextLines(pipeline, inputfile, outputfile);
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
