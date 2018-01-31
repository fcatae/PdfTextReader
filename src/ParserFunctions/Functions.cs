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

            log.Info($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}: Processing file: {document}");
            PdfTextReader.ExamplesAzure.FollowText(g_fileSystem, document);
        }

        [FunctionName("ProcessFolder")]
        public static string ProcessFolder([HttpTrigger]HttpRequest request,
            [Queue("tasks")] ICollector<Model.Pdf> testQueue)
        {
            string year = request.Query["year"];

            if (year == null)
                return "'folder' parameter not specified";

            //var files = g_fileSystem.GetFolder(folder);

            //var files = g_fileSystem.GetFolder(folder); .EnumerateFiles(folder).ToList();

            //foreach (var file in files)
            //{
            //    if(file.ToLower().EndsWith(".pdf"))
            //    {
            //        string fileWithoutExtension = file.Substring(0, file.Length - 4);

            //        testQueue.Add(new Model.Pdf { Name = $"{year}/{fileWithoutExtension}" });
            //    }                
            //}

            return "done";
        }

        [FunctionName("TestListFiles")]
        public static string TestListFiles([HttpTrigger]HttpRequest request,
            [Queue("test")] ICollector<Model.Pdf> testQueue)
        {
            const string INPUT_PATH = "wasb://input/pdf/";

            string year = request.Query[$"{nameof(year)}"];

            if (year == null)
                return $"'{nameof(year)}' parameter not specified";

            string folderName = year.Replace('|', '/');

            var folder = g_fileSystem.GetFolder(INPUT_PATH).GetFolder(folderName);

            foreach(var file in GetFilesRecursive(folder))
            {
                if( file.Name.ToLower().EndsWith(".pdf") )
                {
                    string basepath = file.Path.Substring(INPUT_PATH.Length);
                    string folderPath = basepath.Substring(0, basepath.Length - file.Name.Length).Trim('/');

                    testQueue.Add(new Model.Pdf { Name = file.Name, Path = folderPath });
                }
            }
            
            return "done";
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
            //string document = request.Query["year"];

            return new Model.Pdf { Name = "p40" };
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
