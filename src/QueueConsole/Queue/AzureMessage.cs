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
    }

    public class AzureQueueMessage : IQueueMessage
    {
        public readonly CloudQueueMessage InternalMessage;

        public AzureQueueMessage(CloudQueueMessage message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            InternalMessage = message;
        }

        public string Content => InternalMessage.AsString;
    }
}
