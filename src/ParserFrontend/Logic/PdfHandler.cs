using PdfTextReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ParserFrontend.Logic
{
    public class PdfHandler
    {
        private readonly IVirtualFS _virtualFS;

        public PdfHandler(IVirtualFS virtualFS)
        {
            if (virtualFS == null)
                throw new ArgumentNullException(nameof(virtualFS));

            this._virtualFS = virtualFS;
        }

        public string CreatePdfFile(string filename, string inputname, Stream origin)
        {
            const string PDF = "pdf";
            string basename = GetBasename(filename);

            CheckValidPdfName(filename);

            using (var writer = _virtualFS.OpenWriter($"{inputname}/{basename}.{PDF}"))
            {
                origin.CopyTo(writer);
            }

            return basename;
        }

        string GetBasename(string filename)
        {
            return Path.GetFileNameWithoutExtension(filename);
        }

        void CheckValidPdfName(string filename)
        {
            if (String.Compare(Path.GetExtension(filename), ".PDF", ignoreCase: true) != 0)
                throw new InvalidOperationException("File is not a PDF file");
        }

        public string Process(string basename, string inputfolder, string outputfolder)
        {
            if (basename == null)
                throw new ArgumentNullException(nameof(basename));

            PdfTextReader.ExamplesWeb.RunParserPDF(_virtualFS, basename, inputfolder, outputfolder);

            return $"/files/output/{basename}/parser-output.pdf";
        }
    }
}
