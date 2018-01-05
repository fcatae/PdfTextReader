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

        public void EnumFiles(string input, Func<string,string> outputNameFunc, Action<PipelineInputPdf> callback)
        {
            var inputDirectory = new DirectoryInfo(".");
            
            foreach (var f in inputDirectory.EnumerateFiles(input))
            {
                string inputfile = f.FullName;
                string filename = Path.GetFileNameWithoutExtension(inputfile);

                if (filename.EndsWith("-output"))
                    continue;

                string outputPath = outputNameFunc(filename);

                using (var pdf = Input(inputfile).Output(outputPath))
                {
                    try
                    {
                        callback(pdf);
                    }
                    catch(Exception ex) {
                        Console.WriteLine("===============================");
                        Console.WriteLine($"{ex.Message}");
                        Console.WriteLine("===============================");
                        Console.WriteLine($"{ex.ToString()}");
                        Console.WriteLine("");
                    }                    
                }
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
