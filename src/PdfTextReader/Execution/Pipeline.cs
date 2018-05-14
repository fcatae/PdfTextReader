using System;
using System.Collections.Generic;
using PdfTextReader.Base;
using PdfTextReader.TextStructures;
using System.Linq;
using PdfTextReader.Configuration;

namespace PdfTextReader.Execution
{
    class Pipeline : IDisposable
    {
        private IConfigurationStore _configStore;
        private string _inputFilename = null;
        private PipelineInputPdf _activeContext;
        private PipelineInputCache<IProcessBlockData> _cache = new PipelineInputCache<IProcessBlockData>();
        private PipelineFactory _factory;

        public Pipeline()
        {
            _factory = new PipelineFactory();
        }

        public Pipeline(PipelineFactory factory, IConfigurationStore configurationStore)
        {
            _factory = factory;
            _configStore = configurationStore;
        }

        public Pipeline Config<T>(string filename, bool optional)
            where T: class, IExecutionConfiguration
        {
            var config = _factory.CreateGlobalInstance<T>();

            string content = _configStore.Get(filename);

            if (optional == false && content == null)
                PdfReaderException.AlwaysThrow("File not found: " + filename);

            config.Init(content);

            return this;
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
