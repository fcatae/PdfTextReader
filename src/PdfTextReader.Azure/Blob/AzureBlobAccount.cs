using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Azure.Blob
{
    class AzureBlobAccount : AzureBlobFolder
    {
        CloudBlobClient _client;

        public AzureBlobAccount(AzureBlobRef parent, string accountName, string connectionString)
            : this(parent, accountName, GetClient(connectionString))
        {            
        }

        AzureBlobAccount(AzureBlobRef parent, string accountName, CloudBlobClient client) 
            : base(parent, accountName, client.BaseUri)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            _client = client;
        }

        static CloudBlobClient GetClient(string connectionString)
        {
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var client = storageAccount.CreateCloudBlobClient();

            return client;
        }

        CloudBlobContainer GetContainer(string containerName)
        {
            return _client.GetContainerReference(containerName); 
        }

        protected override AzureBlobFolder GetChildFolder(string name)
        {
            var container = GetContainer(name);

            var folder = new AzureBlobContainer(this, name, container);

            return folder;
        }

        protected override AzureBlobFileBlock GetChildFile(string name)
        {
            throw new System.IO.FileNotFoundException($"'{this.Path}' is a storage account, not a file");
        }

        public override IEnumerable<IAzureBlob> EnumItems()
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

        public override bool Exists()
        {
            return true;
        }
    }
}
