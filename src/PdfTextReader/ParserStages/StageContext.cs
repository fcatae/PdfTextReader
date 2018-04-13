using PdfTextReader.Execution;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.ParserStages
{
    class StageContext : IDisposable
    {
        private Pipeline _pipeline;

        public StageContext(string basename)
        {
            this._pipeline = new Pipeline();
            this.Basename = basename;
        }
        
        public string Basename { get; private set; }
        public string InputFolder => "input";
        public string OutputFolder => "output";
        public string InputFilePrefix => $"{InputFolder}/{Basename}";
        public string OutputFilePrefix => $"{OutputFolder}/{Basename}/{Basename}";

        public Pipeline GetPipeline()
        {
            if (_pipeline == null)
                throw new ArgumentNullException(nameof(_pipeline));

            return _pipeline;
        }

        public void Dispose()
        {
            if( _pipeline != null )
            {
                _pipeline.Dispose();
                _pipeline = null;
            }
        }
    }
}
