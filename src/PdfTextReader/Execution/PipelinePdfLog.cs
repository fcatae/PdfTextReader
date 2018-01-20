using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.Execution
{
    class PipelinePdfLog
    {
        List<PipelinePdfLogEntry> _log = new List<PipelinePdfLogEntry>();

        class PipelinePdfLogEntry
        {
            public int PageNumber;
            public Type Component;
            public string Message;
        }

        public void LogCheck(int pageNumber, Type component, string message)
        {
            _log.Add(new PipelinePdfLogEntry()
            {
                PageNumber = pageNumber,
                Component = component,
                Message = message
            });
        }

        public void SaveErrors(string inputfile, string outputfile)
        {

        }

        public IEnumerable<int> GetErrors()
        {
            return _log.Select(t => t.PageNumber).Distinct().OrderBy(t => t);
        }
    }
}
