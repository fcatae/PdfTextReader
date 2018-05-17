using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace QueueConsole.Queue
{
    public class AzureQueue : IStorageQueue
    {

        CloudStorageAccount _storageAccount;
        CloudQueueClient _queueClient;
        CloudQueue _queue;

        public AzureQueue(string connectionString, string queueName)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("connectionString is empty");
            if (string.IsNullOrWhiteSpace(queueName)) throw new ArgumentException("queueName is empty");

            try
            {
                _storageAccount = CloudStorageAccount.Parse(connectionString);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error parsing the storage connection string {connectionString}: {ex.Message}");
            }

            _queueClient = _storageAccount.CreateCloudQueueClient();
            _queue = CreateQueueAsync(queueName).GetAwaiter().GetResult();

            if (_queue == null)
                throw new Exception("Queue reference is null!");
        }

        public async Task<CloudQueueMessage> AddMessageAsync(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) throw new ArgumentException("message is null!");

            var cloudMessage = new CloudQueueMessage(message);

            await _queue.AddMessageAsync(cloudMessage);

            return cloudMessage;
        }

        private async Task<CloudQueue> CreateQueueAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is empty!");

            var queue = _queueClient.GetQueueReference(name);

            await queue.CreateIfNotExistsAsync();

            return queue;
        }

        public async Task DequeueMessageAsync(CloudQueueMessage message)
        {
            if (message == null) throw new Exception("message is null!");

            await _queue.DeleteMessageAsync(message);
        }


        public async Task<CloudQueueMessage> GetMessageAsync()
        {
            return await _queue.GetMessageAsync();
        }

        public async Task<CloudQueueMessage> PeekMessageAsync()
        {
            return await _queue.PeekMessageAsync();
        }
    }
}
