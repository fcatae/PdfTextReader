using PdfTextReader.Execution;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.ParserStages
{
    class StageContext : IDisposable
    {
        private PipelineFactory _factoryContext;
        private Pipeline _pipeline;
        private object _pipelineText;
        private Dictionary<string, string> _outputFiles = new Dictionary<string, string>();

        public StageContext(string basename) : this(basename, "input", "output")
        {
        }

        public StageContext(string basename, string inputfolder, string outputfolder)
        {
            this._factoryContext = new PipelineFactory();
            this._pipeline = new Pipeline(_factoryContext);
            this.Basename = basename;
            InputFolder = inputfolder;
            OutputFolder = outputfolder;
        }

        public string Basename { get; private set; }
        public string InputFolder { get; private set; }
        public string OutputFolder { get; private set; }
        public string InputFilePrefix => $"{InputFolder}/{Basename}";
        public string OutputFilePrefix => $"{OutputFolder}/{Basename}/{Basename}";
        
        public Pipeline GetPipeline()
        {
            if (_pipeline == null)
                throw new ArgumentNullException(nameof(_pipeline));

            return _pipeline;
        }

        public PipelineText<T> GetPipelineText<T>()
        {
            if (_pipelineText == null)
                throw new ArgumentNullException(nameof(_pipelineText));

            return (PipelineText<T>)_pipelineText;
        }

        public void SetPipelineText<T>(PipelineText<T> pipelineText)
        {
            if (pipelineText == null)
                throw new ArgumentNullException(nameof(pipelineText));

            this._pipelineText = pipelineText;
        }

        public void AddOutput(string name, string link)
        {
            _outputFiles.Add(name, link);
        }

        public string GetOutput(string name)
        {
            return _outputFiles[name];
        }

        public Dictionary<string,string> FileListOutput
        {
            get { return _outputFiles; }
        }

        public void Dispose()
        {
            if( _pipeline != null )
            {
                _pipeline.Dispose();
                _pipeline = null;
            }

            if( _factoryContext != null )
            {
                _factoryContext.Dispose();
                _factoryContext = null;
            }
        }
    }
}
