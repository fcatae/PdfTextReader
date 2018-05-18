using PdfTextReader.Azure.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParserFrontend.Logic
{
    public class JobManager
    {
        PdfHandler _pdfHandler;
        OutputFiles _outputFiles;
        AzureQueue _queue;

        public JobManager(AzureQueue queue, AccessManager amgr)
        {
            var vfs = amgr.GetFullAccessFileSystem();

            _pdfHandler = new PdfHandler(vfs);
            _outputFiles = new OutputFiles(vfs);

            _queue = queue;
        }
        
        public void Process()
        {
            var msg = _queue.TryGetMessageAsync().Result;

            if (msg == null)
                return;

            Process(msg.Content);

            msg.Done();
        }

        void Process(string name)
        {
            var fileList = _pdfHandler.Process(name, "input", "output");

            _outputFiles.Save(name, fileList);
        }
    }
}
