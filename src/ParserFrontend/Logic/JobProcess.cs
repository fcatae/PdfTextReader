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
        private CopyFiles _copyFiles;

        public JobProcess(AccessManager amgr, CopyFiles copyFiles)
        {
            var vfs = amgr.GetFullAccessFileSystem();

            _pdfHandler = new PdfHandler(vfs);
            _outputFiles = new OutputFiles(vfs);

            _copyFiles = copyFiles;
        }

        public void Process(string name)
        {
            if( _copyFiles != null )
            {
                _copyFiles.EnsureFile($"input/{name}.pdf", name);
            }

            var fileList = _pdfHandler.Process(name, "input", "output");

            _outputFiles.Save(name, fileList);
        }
    }
}
