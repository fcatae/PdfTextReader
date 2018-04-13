using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using PdfTextReader.Azure;
using System.Collections.Generic;

namespace PdfToImageFunction
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static void Run([QueueTrigger("pdftoimage", Connection = "queuestorage")]string pdfFileUrl, TraceWriter log)
        {            

            var gs = $@"{Environment.GetEnvironmentVariable("HOME")}\site\wwwroot\gs\gswin64c.exe";
            var tempFolder = $@"{Environment.GetEnvironmentVariable("HOME")}\site\wwwroot\temp";

            log.Info($"Downloading resource - {pdfFileUrl}");

            var pdfTempFile = string.Empty;            

            try
            {
                pdfTempFile = DownloadTempPdf(pdfFileUrl, tempFolder);
            }
            catch (Exception ex)
            {
                log.Error($"Error: {ex.Message}");
                log.Error($"Error: {ex.InnerException}");
            }

            Stream[] pdfPageImageList = null;

            using (var pdfInput = File.OpenRead(pdfTempFile))
            {
                log.Info("Generating Image stream array");
                PdfImageConverter imageConverter = new PdfImageConverter(gs, tempFolder, "204.8");

                try
                {
                    //The array of streams will respect the page number-1, page 1 equal index 0;
                    imageConverter.GenerateImage(pdfInput, ref pdfPageImageList);
                }
                catch (Exception ex) {
                    log.Error($"Error generating pdf images {ex.Message}");
                }

                
            }

            if (pdfPageImageList == null)
                log.Info($"No Pages was generated!");

            else
            {

                log.Info($"Uploading Images to final Storage Account");


                FileInfo info = new FileInfo(pdfTempFile);

                try
                {
                    UploadImages(pdfPageImageList, info.Name.ToUpper().Replace(".PDF", ""));
                }
                catch (Exception ex) {
                    log.Error($"Error Uploading Images: {ex.Message}");
                }
                
            }

            try
            {
                File.Delete(pdfTempFile);
            }
            catch (Exception ex)
            {
                log.Error($"Error trying to delete the temp file {pdfTempFile}: {ex.Message}");
            }

        }

        private static void UploadImages(Stream[] pdfPageImageList, string filename)
        {
            //Upload Pages in Patch
            var container = GetContainer(Environment.GetEnvironmentVariable("imagestorage"), Environment.GetEnvironmentVariable("imagepdf"));

            container.CreateIfNotExistsAsync().GetAwaiter().GetResult();

            container.SetPermissionsAsync(new BlobContainerPermissions()
            {
                PublicAccess = BlobContainerPublicAccessType.Container
            }).GetAwaiter().GetResult();


            var tasks = new List<Task>();
            for(int page = 0; page < pdfPageImageList.Length; page++)
            {
                var blobName = $"{filename}/page_{page+1}.jpg";
                var blob = container.GetBlockBlobReference(blobName);
                tasks.Add(blob.UploadFromStreamAsync(pdfPageImageList[page]));
            }
            Task.WaitAll(tasks.ToArray());      

        }

        private static CloudBlobContainer GetContainer(string connectionString, string containerName)
        {
            string conn = connectionString;

            var storageAccount = CloudStorageAccount.Parse(conn);
            var client = storageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference(containerName);

            return container;
        }

        private static string DownloadTempPdf(string pdfFileUrl, string tempFolder)
        {
            var fileUri = new Uri(pdfFileUrl);
            var fileName = $@"{tempFolder}\{fileUri.Segments[fileUri.Segments.Length-1]}";

            using (var client = new WebClient())
            {
                client.DownloadFile(fileUri, fileName);
            }

            return fileName;
        }
    }
}
