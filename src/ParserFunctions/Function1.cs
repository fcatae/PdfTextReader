using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace ParserFunctions
{
    public static class Function1
    {
        static string g_storageAccount;
        static AzureFS g_fileSystem;

        static Function1()
        {
            string inputStorage = Environment.GetEnvironmentVariable("PDFTEXTREADER_PDF");
            string inputContainer = "pdf";
            string outputStorage = Environment.GetEnvironmentVariable("PDFTEXTREADER_OUTPUT");
            string outputContainer = "output";

            var input = new AzureBlob(inputStorage, inputContainer);
            var output = new AzureBlob(outputStorage, outputContainer);

            g_fileSystem = new AzureFS(input, output);
        }

        [FunctionName("Function1")]
        public static void Run([QueueTrigger("tasks")]dynamic myQueueItem, TraceWriter log)
        {
            log.Info($"C# Queue trigger function processed: {myQueueItem}");
            string document = myQueueItem.name;

            PdfTextReader.ExamplesAzure.FollowText(g_fileSystem, document);
        }
    }
}
