using System;
using System.Collections.Generic;
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
