using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace QueueConsole.Queue
{
    public interface IQueueMessage
    {
        string Content { get; }
        void Done();
    }

    public class AzureQueueMessage : IQueueMessage
    {
        private readonly CloudQueueMessage _internalMessage;
        private readonly AzureQueue _azureQueue;

        public AzureQueueMessage(AzureQueue queue, CloudQueueMessage message)
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));

            if (message == null)
                throw new ArgumentNullException(nameof(message));

            _azureQueue = queue;
            _internalMessage = message;
        }

        public CloudQueueMessage InternalMessage => _internalMessage;

        public string Content => _internalMessage.AsString;

        public void Done()
        {
            _azureQueue.DequeueMessageAsync(this).Wait();
        }
    }
}
