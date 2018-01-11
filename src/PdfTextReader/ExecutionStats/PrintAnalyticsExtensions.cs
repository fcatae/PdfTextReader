using PdfTextReader.Base;
using PdfTextReader.Execution;
using PdfTextReader.Parser;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.ExecutionStats
{
    static class PrintAnalyticsExtensions
    {
        public static PipelineText<TextLine> PrintAnalytics(this PipelineText<TextLine> pipelineText, string filename)
        {
            return pipelineText.Log<PrintAnalytics.ShowTextLine>(filename);
        }

        public static PipelineText<TextStructure> PrintAnalytics(this PipelineText<TextStructure> pipelineText, string filename)
        {
            return pipelineText.Log<PrintAnalytics.ShowTextStructure>(filename);
        }

        public static PipelineText<Conteudo> PrintAnalytics(this PipelineText<Conteudo> pipelineText, string filename)
        {
            return pipelineText.Log<PrintAnalytics.ShowConteudo>(filename);
        }

    }
}
