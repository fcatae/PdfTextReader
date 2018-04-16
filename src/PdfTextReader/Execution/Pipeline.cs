using System;
using System.Collections.Generic;
using PdfTextReader.Base;
using PdfTextReader.TextStructures;
using System.Linq;

namespace PdfTextReader.Execution
{
    class Pipeline : IDisposable
    {
        private string _inputFilename = null;
        private PipelineInputPdf _activeContext;
        private PipelineInputCache<IProcessBlockData> _cache = new PipelineInputCache<IProcessBlockData>();
        private PipelineFactoryContext _factory;

        public Pipeline()
        {
            _factory = new PipelineFactoryContext();
        }

        public Pipeline(PipelineFactoryContext factory)
        {
            _factory = factory;
        }

        public PipelineInputPdf Input(string filename)
        {
            var context = new PipelineInputPdf(filename, _factory, _cache);

            this._activeContext = context;
            this._inputFilename = filename;

            return context;
        }

        public string Filename => _inputFilename;
        public PipelineStats Statistics => _activeContext.Statistics;
        public TransformIndexTree Index => _activeContext.Index;

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
