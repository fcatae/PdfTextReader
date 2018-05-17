using QueueConsole.Queue;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QueueConsole
{
    class MainConsole
    {
        public void Run(string queueSas)
        {
            RunWriterAsync(queueSas).Wait();

            Console.WriteLine("Read messages:");
            RunReaderAsync(queueSas).Wait();
        }

        public async Task RunWriterAsync(string queueSas)
        {
            var azQueue = await AzureQueue.OpenAsync(queueSas);

            while(true)
            {
                string text = Console.ReadLine();

                if (String.IsNullOrEmpty(text))
                    break;

                await azQueue.AddMessageAsync(text);
            }            
        }
        public async Task RunReaderAsync(string queueSas)
        {
            var azQueue = await AzureQueue.OpenAsync(queueSas);

            while (true)
            {
                var message = await azQueue.TryGetMessageAsync();

                if (message == null)
                    break;

                Console.WriteLine($"message: {message.Content}");

                await azQueue.DequeueMessageAsync(message);
            }
        }
    }
}
