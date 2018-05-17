using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace QueueConsole.Queue
{
    public class AzureQueue : IStorageQueue
    {
        CloudQueue _queue;
        TimeSpan _visibilityTimeout = TimeSpan.FromMinutes(5);

        readonly QueueRequestOptions DefaultQueueRequestOptions = new QueueRequestOptions();

        private AzureQueue(CloudQueue queue)
        {
            if (queue == null) throw new ArgumentException("CloudQueue is empty");

            _queue = queue;
        }

        public static Task<AzureQueue> OpenAsync(string connectionString, string queueName)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("connectionString is empty");
            if (string.IsNullOrWhiteSpace(queueName)) throw new ArgumentException("queueName is empty");

            var storageAccount = CloudStorageAccount.Parse(connectionString);

            var queueClient = storageAccount.CreateCloudQueueClient();
            
            var nativeQueue = queueClient.GetQueueReference(queueName);

            return Task.FromResult(new AzureQueue(nativeQueue));
        }

        public static Task<AzureQueue> OpenAsync(string queueUrlSas)
        {
            var nativeQueue = new CloudQueue(new Uri(queueUrlSas));
            
            return Task.FromResult(new AzureQueue(nativeQueue));
        }

        public static async Task<AzureQueue> CreateAsync(string queueUrlSas)
        {
            var queue = await OpenAsync(queueUrlSas);

            await queue.EnsureQueueCreatedAsync();

            return queue;
        }

        public static async Task<AzureQueue> CreateAsync(string connectionString, string queueName)
        {
            var queue = await OpenAsync(connectionString, queueName);

            await queue.EnsureQueueCreatedAsync();

            return queue;
        }

        private async Task EnsureQueueCreatedAsync()
        {
            await _queue.CreateIfNotExistsAsync();
        }

        public async Task AddMessageAsync(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) throw new ArgumentException("message is null!");

            var cloudMessage = new CloudQueueMessage(message);

            await _queue.AddMessageAsync(cloudMessage);
        }

        public async Task DequeueMessageAsync(IQueueMessage queueMessage)
        {
            var message = queueMessage as AzureQueueMessage;

            if (message == null) throw new Exception("message is null!");

            await _queue.DeleteMessageAsync(message.InternalMessage);
        }
        
        public async Task<IQueueMessage> GetMessageAsync()
        {
            var message = await _queue.GetMessageAsync(_visibilityTimeout, DefaultQueueRequestOptions, null);

            return new AzureQueueMessage(message);
        }

        public async Task<IQueueMessage> PeekMessageAsync()
        {
            var message = await _queue.PeekMessageAsync();

            return new AzureQueueMessage(message);
        }
    }
}
