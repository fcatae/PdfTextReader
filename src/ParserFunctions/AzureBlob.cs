using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Diagnostics;

namespace ParserFunctions
{
    class AzureBlob
    {
        string _connectionString = null;
        string _containerName = null;

        public AzureBlob(string connectionString, string containerName)
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

        // synchronous version
        [DebuggerHidden]
        public Stream GetStreamWriter(string filename) => GetStreamWriterAsync(filename).Result;
        [DebuggerHidden]
        public Stream GetStreamReader(string filename) => GetStreamReaderAsync(filename).Result;

        public async Task<Stream> GetStreamWriterAsync(string filename)
        {
            var container = GetContainer();
            var blockBlob = container.GetBlockBlobReference(filename);

            return await blockBlob.OpenWriteAsync();
        }
        
        public Task<Stream> GetStreamReaderAsync(string filename)
        {
            var container = GetContainer();
            var blockBlob = container.GetBlockBlobReference(filename);

            return blockBlob.OpenReadAsync();
        }
    }
}
