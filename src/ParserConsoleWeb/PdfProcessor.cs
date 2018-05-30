using System;
using System.Collections.Generic;
using System.Text;
using PdfTextReader;

namespace ParserConsoleWeb
{
    class PdfProcessor
    {
        IVirtualFS _vfs;

        public PdfProcessor(IVirtualFS vfs)
        {
            _vfs = vfs;
        }

        public void Process(string basename)
        {
            ExampleStages.RunParserPDF(_vfs, basename, "input", "output");
        }
    }
}
