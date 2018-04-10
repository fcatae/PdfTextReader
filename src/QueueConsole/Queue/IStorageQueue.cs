using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QueueConsole.Queue
{
    public interface IStorageQueue
    {        

        Task<CloudQueueMessage> AddMessageAsync(string message);

        Task<CloudQueueMessage> PeekMessageAsync();

        Task<CloudQueueMessage> GetMessageAsync();

        Task DequeueMessageAsync(CloudQueueMessage message);
        

    }
}
