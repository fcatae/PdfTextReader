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

        public StageContext(string basename)
        {
            this._factoryContext = new PipelineFactory();
            this._pipeline = new Pipeline(_factoryContext);
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
