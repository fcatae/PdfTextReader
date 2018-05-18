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

        public JobManager(AzureQueue queue)
        {
            _queue = queue;
        }

        public async Task<string[]> GetAsync()
        {
            List<string> messages = new List<string>();

            for(int i=0; i<10; i++)
            {
                var msg = await _queue.TryGetMessageAsync();

                if (msg == null)
                    break;

                messages.Add(msg.Content);
            }

            return messages.ToArray();
        }
    }
}
