using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace ParserAzure
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static void Run([QueueTrigger("inputpdf", Connection = "PDFTEXTREADER_STORAGE")]string myQueueItem, TraceWriter log)
        {
            log.Info($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
