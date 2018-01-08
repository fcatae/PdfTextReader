using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PdfTextReader.Execution
{
    class Pipeline : IDisposable
    {
        private PipelineInputPdf _activeContext;

        public PipelineInputPdf Input(string filename)
        {
            var context = new PipelineInputPdf(filename);

            this._activeContext = context;

            return context;
        }

        public void EnumFiles(string input, Action<string> callback)
        {
            var inputDirectory = new DirectoryInfo(".");

            string inputPattern = Path.Combine(input, "*.pdf");

            foreach (var f in inputDirectory.EnumerateFiles(inputPattern))
            {
                string inputfile = f.FullName;
                string filename = Path.GetFileNameWithoutExtension(inputfile);

                if (filename.EndsWith("-output"))
                    continue;

                string name = Path.Combine(input, filename);

                callback(name);
            }
        }

        public void GetResults()
        {
            //_activeContext.CurrentPage
        }

        public void Done()
        {
            Dispose();
        }

        public void Dispose()
        {
            var disposable = _activeContext as IDisposable;

            if ( disposable != null )
            {
                disposable.Dispose();
            }

            _activeContext = null;
        }
    }
}
