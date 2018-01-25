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
        static string errors;

        public static void Process(string basename, string inputfolder, string outputfolder)
        {
            logDir = Directory.CreateDirectory($"{outputfolder}/Log").FullName;
            xmlDir = Directory.CreateDirectory($"{outputfolder}/XMLs").FullName;
            errors = Directory.CreateDirectory($"{outputfolder}/PDF-Errors").FullName;

            PdfReaderException.ContinueOnException();

            //ExamplesWork.PrintAllSteps(basename, inputfolder, outputfolder);

            var conteudos = GetTextLines(basename, inputfolder, outputfolder, out Execution.Pipeline pipeline)
                                .Log<AnalyzeLines>($"{logDir}/{basename}-lines.txt")
                            .ConvertText<CreateTextLineIndex, TextLine>()
                            .ConvertText<CreateStructures, TextStructure>()
                                .Log<AnalyzeStructuresCentral>($"{logDir}/{basename}-central.txt")
                                .ShowPdf<ShowStructureCentral>($"{logDir}/{basename}-show-central.pdf")
                            .PrintAnalytics($"{logDir}/{basename}-print-analytics.txt")
                            .ConvertText<CreateTextSegments, TextSegment>()
                                .Log<AnalyzeSegmentTitles>($"{logDir}/{basename}-tree.txt")
                                .Log<AnalyzeSegmentStats>($"{logDir}/{basename}-segments-stats.txt")
                                .Log<AnalyzeSegments2>($"{logDir}/{basename}-segments.csv")
                            .ConvertText<CreateTreeSegments, TextSegment>()
                                .Log<AnalyzeTreeStructure>($"{logDir}/{basename}-tree-data.txt")
                            .ConvertText<TransformConteudo, Conteudo>()
                            .ConvertText<AggregateAnexo, Conteudo>()
                            .ConvertText<AggregateSingularBody, Conteudo>()
                            .ToList();
            //Create XML
            var createArticle = new TransformArtigo();
            var artigos = createArticle.Create(conteudos);
            createArticle.CreateXML(artigos, xmlDir, basename);

            pipeline.ExtractOutput<ShowParserWarnings>($"{errors}/{basename}-parser-errors.pdf");

            pipeline.Done();

            var validator = new ProgramValidatorXML();
            validator.ValidateArticle(outputfolder);
        }

        static PipelineText<TextLine> GetTextLines(string basename, string inputfolder, string outputfolder, out Execution.Pipeline pipeline)
        {
            string inputfile = $"{inputfolder}/{basename}.pdf";
            string outputfile = $"{outputfolder}/{basename}-parser.pdf";

            return Examples.GetTextLines(inputfile, outputfile, out pipeline);
        }
    }
}
