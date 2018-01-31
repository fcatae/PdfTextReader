using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.AspNetCore.Http;
using System.Linq;
using PdfTextReader.Azure;
using System.Collections.Generic;

namespace ParserFunctions
{
    public static class Functions
    {
        const string INPUT_PATH = "wasb://input/pdf/";
        const string OUTPUT_PATH = "wasb://output/output/";

        static AzureFS g_fileSystem;

        static Functions()
        {
            string inputStorage = Environment.GetEnvironmentVariable("PDFTEXTREADER_PDF");
            string outputStorage = Environment.GetEnvironmentVariable("PDFTEXTREADER_OUTPUT");
            
            g_fileSystem = new AzureFS(inputStorage, outputStorage);
        }

        [FunctionName("ProcessPdf")]
        public static void ProcessPdf([QueueTrigger("tasks")]Model.Pdf pdf, TraceWriter log)
        {
            string document = pdf.Name;
            string inputfolder = $"{INPUT_PATH}{pdf.Path}";
            string outputfolder = $"{OUTPUT_PATH}{pdf.Path}";

            log.Info($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}: Processing file: {document}, inputfolder={inputfolder}, outputfolder={outputfolder}");

            PdfTextReader.ExamplesAzure.RunParserPDF(g_fileSystem, document, inputfolder, outputfolder);
        }

        [FunctionName("ProcessFolder")]
        public static string ProcessFolder([HttpTrigger]HttpRequest request,
            [Queue("tasks")] ICollector<Model.Pdf> testQueue)
        {
            string year = request.Query[$"{nameof(year)}"];

            if (year == null)
                return $"'{nameof(year)}' parameter not specified";

            string folderName = year.Replace('|', '/');

            foreach (var pdf in GetInput(folderName))
            {
                testQueue.Add(pdf);
            }

            return "done";
        }

        [FunctionName("TestListFiles")]
        public static string TestListFiles([HttpTrigger]HttpRequest request,
            [Queue("test")] ICollector<Model.Pdf> testQueue)
        {
            string year = request.Query[$"{nameof(year)}"];

            if (year == null)
                return $"'{nameof(year)}' parameter not specified";

            string folderName = year.Replace('|', '/');

            foreach(var pdf in GetInput(folderName))
            {
                testQueue.Add(pdf);
            }

            return "done";
        }
        
        static IEnumerable<Model.Pdf> GetInput(string folderName)
        {
            var folder = g_fileSystem.GetFolder(INPUT_PATH).GetFolder(folderName);

            foreach (var file in GetFilesRecursive(folder))
            {
                if (file.Name.ToLower().EndsWith(".pdf"))
                {
                    string basepath = file.Path.Substring(INPUT_PATH.Length);
                    string folderPath = basepath.Substring(0, basepath.Length - file.Name.Length).Trim('/');

                    yield return new Model.Pdf { Name = file.Name, Path = folderPath };
                }
            }
        }

        static IEnumerable<IAzureBlobFile> GetFilesRecursive(IAzureBlobFolder folder)
        {
            var items = folder.EnumItems();

            foreach(var it in items)
            {
                if (it is IAzureBlobFile)
                    yield return (IAzureBlobFile)it;

                if (it is IAzureBlobFolder)
                {
                    var childFolder = (IAzureBlobFolder)it;
                    var recursive_items = GetFilesRecursive(childFolder);

                    foreach( var rec_it in recursive_items )
                    {
                        yield return rec_it;
                    }
                }
            }            
        }

        [FunctionName("Ping")]
        [return: Queue("tasks")]
        public static Model.Pdf Ping([HttpTrigger]HttpRequest request, TraceWriter log)
        {
            return new Model.Pdf { Name = "p40" , Path = "test"};
        }
        
        [FunctionName("Test")]
        public static string Test([HttpTrigger]HttpRequest request)
        {
            bool inputOk;
            bool outputOk;

            try { TestAzureBlob.Run(g_fileSystem, "wasb://input/pdf/test.txt"); inputOk = true; }
            catch { inputOk = false; }

            try { TestAzureBlob.Run(g_fileSystem, "wasb://output/test/test.txt"); outputOk = true; }
            catch { outputOk = false; }

            return $"Input={inputOk}, Output={outputOk}";
        }
    }
}
