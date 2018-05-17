using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QueueConsole.Queue
{
    public interface IStorageQueue
    {
        Task AddMessageAsync(string message);

        Task<IQueueMessage> PeekMessageAsync();

        Task<IQueueMessage> TryGetMessageAsync();

        Task DequeueMessageAsync(IQueueMessage message);
    }
}
