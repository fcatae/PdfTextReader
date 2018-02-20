using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PdfTextReader.Base;
using PdfTextReader.TextStructures;
using System.Linq;

namespace PdfTextReader.Execution
{
    class Pipeline : IDisposable
    {
        private string _inputFilename = null;
        private PipelineInputPdf _activeContext;

        public PipelineInputPdf Input(string filename)
        {
            var context = new PipelineInputPdf(filename);

            this._activeContext = context;
            this._inputFilename = filename;

            return context;
        }

        public string Filename => _inputFilename;
        public PipelineStats Statistics => _activeContext.Statistics;
        public TransformIndexTree Index => _activeContext.Index;

        public void EnumFiles(string input, Action<string> callback)
        {
            var inputDirectory = new DirectoryInfo(".");

            string inputPattern = Path.Combine(input, "*.pdf");

            foreach (var f in inputDirectory.EnumerateFiles(inputPattern))
            {
                string inputfile = f.FullName;
                string filename = Path.GetFileNameWithoutExtension(inputfile);

                if (filename.StartsWith("~"))
                    continue;

                if (filename.EndsWith("-output"))
                    continue;

                string name = Path.Combine(input, filename);

                callback(name);
            }
        }
        public void EnumFolders(string input, Action<string> callback)
        {
            var inputDirectory = new DirectoryInfo(input);

            foreach (var d in inputDirectory.EnumerateDirectories())
            {
                foreach(var f in d.EnumerateFiles("*.pdf"))
                {
                    string inputfile = f.FullName;
                    string foldername = f.DirectoryName;
                    string filename = Path.GetFileNameWithoutExtension(inputfile);

                    if (filename.StartsWith("~"))
                        continue;

                    if (filename.EndsWith("-output"))
                        continue;

                    string name = Path.Combine(foldername, filename);

                    callback(name);
                }

            }
        }

        public void SaveOk(string outputname)
        {
            _activeContext.SaveOk(outputname);
        }
        public int SaveErrors(string outputname)
        {
            return _activeContext.SaveErrors(outputname);
        }

        public void ExtractOutput<T>(string filename)
        {
            var showParserWarnings = new ExecutionStats.ShowParserWarnings();

            var pages = showParserWarnings.GetPages(this.Statistics);

            if (pages.Count() > 0)
            {
                _activeContext.ExtractOutputPages(filename, pages);
            }
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
