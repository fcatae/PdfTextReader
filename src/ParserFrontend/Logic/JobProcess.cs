using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParserFrontend.Logic
{
    public class JobProcess
    {
        private PdfHandler _pdfHandler;
        private OutputFiles _outputFiles;

        public JobProcess(AccessManager amgr)
        {
            var vfs = amgr.GetFullAccessFileSystem();

            _pdfHandler = new PdfHandler(vfs);
            _outputFiles = new OutputFiles(vfs);
        }

        public void Process(string name)
        {
            var fileList = _pdfHandler.Process(name, "input", "output");

            _outputFiles.Save(name, fileList);
        }
    }
}
