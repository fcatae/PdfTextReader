using PdfTextReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParserFrontend.Logic
{
    public class InputFiles
    {
        const string PDF_EXTENSION = "*.pdf";

        private readonly WebVirtualFS _webFS;

        public InputFiles(WebVirtualFS webFS)
        {
            this._webFS = webFS; 
        }

        public IEnumerable<string> List()
        {
            return _webFS.ListFiles(PDF_EXTENSION);
        }
    }
}
