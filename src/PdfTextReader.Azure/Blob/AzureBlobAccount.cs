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

        CloudBlobContainer GetContainer(string containerName)
        {
            return _client.GetContainerReference(containerName); 
        }

        public override AzureBlobFolder GetFolder(string name)
        {
            var container = GetContainer(name);

            var folder = new AzureBlobContainer(this, name, container);

            return folder;
        }

        public override IEnumerable<AzureBlobRef> EnumItems()
        {
            BlobContinuationToken token = null;

            do
            {
                var segment = _client.ListContainersSegmentedAsync(token).Result;

                foreach (var container in segment.Results)
                {
                    string name = container.Name;

                    yield return new AzureBlobContainer(this, name, container);
                }

                token = segment.ContinuationToken;

            } while (token != null);
        }
    }
}
