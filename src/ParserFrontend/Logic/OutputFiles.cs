using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ParserFrontend.Logic
{
    public class OutputFiles
    {
        private readonly WebVirtualFS _webFs;

        public OutputFiles()
        {
            this._webFs = new WebVirtualFS();
            
            //return $"/files/output/{basename}/parser-output.pdf";
        }

        public Stream GetOutputFile(string basename)
        {
            return _webFs.OpenReader($"output/{basename}/parser-output.pdf");
        }
    }
}
