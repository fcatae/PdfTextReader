using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace ParserAzure
{
    public static class Function1
    {
        [FunctionName("Function1")]
        [return: Queue("myqueue-output", Connection = "QueueOUT")]
        public static string Run([QueueTrigger("myqueue-items", Connection = "QueueIN")]string myQueueItem, TraceWriter log)
        {
            log.Info($"C# Queue trigger function processed: {myQueueItem}");
            return ($"RETORNO DA PORRA: {myQueueItem}");
        }
    }
}
