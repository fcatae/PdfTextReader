using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace ParserFunctions
{
    public static class Functions
    {
        static AzureFS g_fileSystem;
        static AzureBlob g_input;
        static AzureBlob g_output;

        static Functions()
        {
            string inputStorage = Environment.GetEnvironmentVariable("PDFTEXTREADER_PDF");
            string inputContainer = "pdf";
            string outputStorage = Environment.GetEnvironmentVariable("PDFTEXTREADER_OUTPUT");
            string outputContainer = "output";

            var input = new AzureBlob(inputStorage, inputContainer);
            var output = new AzureBlob(outputStorage, outputContainer);

            g_fileSystem = new AzureFS(input, output);
            g_input = input;
            g_output = output;
        }

        [FunctionName("ProcessPdf")]
        public static void ProcessPdf([QueueTrigger("tasks")]Model.Pdf pdf, TraceWriter log)
        {
            string document = pdf.Name;

            log.Info($"Processing file: {document}");
            PdfTextReader.ExamplesAzure.FollowText(g_fileSystem, document);
        }

        [FunctionName("TestListFiles")]
        public static void TestListFiles([HttpTrigger]HttpRequest request,
            [Queue("test")] ICollector<Model.Pdf> testQueue)
        {
            string folder = ((string)request.Query["folder"]) ?? "";

            var files = g_input.EnumerateFiles(folder).ToList();

            foreach(var file in files)
            {
                testQueue.Add(new Model.Pdf { Name = file });
            }            
        }

        [FunctionName("Ping")]
        [return: Queue("tasks")]
        public static Model.Pdf Ping([HttpTrigger]HttpRequest request, TraceWriter log)
        {
            //string document = request.Query["year"];

            return new Model.Pdf { Name = "p40" };
        }
        
        [FunctionName("Test")]
        public static string Test([HttpTrigger]HttpRequest request)
        {
            bool inputOk;
            bool outputOk;

            try { TestAzureBlob.Run(g_input, "test.txt"); inputOk = true; }
            catch { inputOk = false; }

            try { TestAzureBlob.Run(g_output, "test.txt"); outputOk = true; }
            catch { outputOk = false; }

            return $"Input={inputOk}, Output={outputOk}";
        }
    }
}
