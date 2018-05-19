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
            bool isFullPath = (name.ToLower().Contains(".pdf") || name.Contains("/"));

            if(isFullPath)
            {
                ProcessFullPath(name);
            }
            else
            {
                ProcessBasename(name);
            }
        }

        public void ProcessBasename(string name)
        {
            if( _copyFiles != null )
            {
                _copyFiles.EnsureFile($"input/{name}.pdf", name);
            }

            var fileList = _pdfHandler.Process(name, "input", "output");

            _outputFiles.Save(name, fileList);
        }

        public void ProcessFullPath(string path)
        {
            string basename = GetBaseName(path);

            if (_copyFiles != null)
            {
                _copyFiles.EnsureFileWithPath($"input/{basename}.pdf", basename, path);
            }

            var fileList = _pdfHandler.Process(basename, "input", "output");

            _outputFiles.Save(basename, fileList);
        }

        string GetBaseName(string filepath)
        {
            var comps = filepath.Split("/");
            string filename = comps[comps.Length - 1];

            var comps2 = filename.Split(".");
            string basename = comps2[0];

            return basename;
        }
    }
}
