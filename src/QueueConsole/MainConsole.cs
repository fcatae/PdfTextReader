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
            RunAsync(queueSas).Wait();
        }

        public async Task RunAsync(string queueSas)
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
    }
}
