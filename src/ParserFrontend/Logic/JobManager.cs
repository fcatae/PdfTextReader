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
        JobProcessHttp _jobHttp = new JobProcessHttp();

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

                Console.WriteLine("MessageLoopAsync: " + msg.Content);

                _jobHttp.Process(msg.Content);

                //try { _job.Process(msg.Content); }
                //catch { }                

                msg.Done();
            }
        }
    }
}
