using PdfTextReader.Azure.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParserFrontend.Logic
{
    public class JobManager
    {
        AzureQueue _queue;
        JobProcess _job;

        public JobManager(AzureQueue queue, JobProcess job)
        {
            _queue = queue;
            _job = job;
        }
        
        public async Task MessageLoopAsync()
        {
            while(true)
            {
                var msg = await _queue.TryGetMessageAsync();

                if (msg == null)
                    return;

                try { _job.Process(msg.Content); }
                catch { }                

                msg.Done();
            }
        }
    }
}
