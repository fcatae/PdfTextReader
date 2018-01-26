using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ParserAzure
{
    class BlobHelper2
    {
        string _connectionString = null;
        string _containerName = null;

        public BlobHelper2(string connectionString, string containerName)
        {
            _connectionString = connectionString;
            _containerName = containerName;
        }

        private CloudBlobContainer GetContainer()
        {
            string conn = _connectionString;
            string containerName = _containerName;

            var storageAccount = CloudStorageAccount.Parse(conn);
            var client = storageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference(containerName);
                        
            // container.CreateIfNotExists();

            return container;
        }

        public void Write(string filename, Stream stream)
        {
            var container = GetContainer();
            var blockBlob = container.GetBlockBlobReference(filename);

            blockBlob.UploadFromStream(stream);
        }
        public Stream GetWriter(string filename)
        {
            var container = GetContainer();
            var blockBlob = container.GetBlockBlobReference(filename);

            return blockBlob.OpenWrite();
        }

        public void Read(string filename, Stream stream)
        {
            var container = GetContainer();
            var blockBlob = container.GetBlockBlobReference(filename);

            blockBlob.DownloadToStream(stream);
        }

        public Stream GetReader(string filename, Stream stream)
        {
            var container = GetContainer();
            var blockBlob = container.GetBlockBlobReference(filename);

            return blockBlob.OpenRead();
        }
    }
}
