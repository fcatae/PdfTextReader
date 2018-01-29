using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Azure.Blob
{
    public class AzureBlobAccount : AzureBlobFolder
    {
        CloudBlobClient _client;

        public AzureBlobAccount(string connectionString, string accountName) : base(accountName)
        {
            _client = GetClient(connectionString);
        }

        CloudBlobClient GetClient(string connectionString)
        {
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var client = storageAccount.CreateCloudBlobClient();

            return client;
        }

        public override AzureBlobFolder GetFolder(string folder)
        {
            return base.GetFolder(folder);
        }

        public override AzureBlobRef EnumFiles()
        {

            return base.EnumFiles();
        }
    }
}
