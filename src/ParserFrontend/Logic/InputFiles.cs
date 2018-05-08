using PdfTextReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParserFrontend.Logic
{
    public class InputFiles
    {
        const string PDF_EXTENSION = ".pdf";
        const string PDF_EXTENSION_PATTERN = "*.pdf";

        private readonly IVirtualFS2 _webFS;

        public InputFiles(IVirtualFS2 webFS)
        {
            this._webFS = webFS; 
        }

        public IEnumerable<string> List()
        {
            var filenames = _webFS.ListFiles(PDF_EXTENSION_PATTERN);

            return filenames.Select(n => RemoveExtension(n));
        }

        string RemoveExtension(string name)
        {
            if( name.EndsWith(PDF_EXTENSION, StringComparison.OrdinalIgnoreCase) )
                return name.Substring(0, name.Length - PDF_EXTENSION.Length);

            return name;
        }
    }
}
