using PdfTextReader.Base;
using PdfTextReader.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.ExecutionStats
{
    class ShowParserWarnings
    {
        public IEnumerable<int> GetPages(PipelineStats statistics)
        {
            var layout = (ValidateLayout)statistics.Calculate<ValidateLayout, StatsPageLayout>();
            var overlap = (ValidateOverlap)statistics.Calculate<ValidateOverlap, StatsBlocksOverlapped>();
            var unhandled = (ValidateUnhandledExceptions)statistics.Calculate<ValidateUnhandledExceptions, StatsExceptionHandled>();

            var pagesLayout = layout.GetPageErrors().ToList();
            var pagesOverlap = overlap.GetPageErrors().ToList();
            var pagesUnhandled = unhandled.GetPageErrors().ToList();

            var pages = pagesLayout
                            .Concat(pagesOverlap)
                            .Concat(pagesUnhandled)
                            .Distinct().OrderBy(t => t).ToList();

            return pages;
        }
    }
}
