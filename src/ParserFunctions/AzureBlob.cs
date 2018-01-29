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
        
        public IEnumerable<string> EnumerateFiles(string folder)
        {
            if (folder == null )
                throw new ArgumentNullException(nameof(folder));

            var container = GetContainer();
            var directory = container.GetDirectoryReference(folder);
            
            var files = EnumerateFilesInternal(directory).ToList();

            return MakeRelativePath(files, directory.Uri);
        }

        IEnumerable<string> EnumerateFilesInternal(CloudBlobDirectory folder)
        {
            BlobContinuationToken token = null;

            do
            {
                var segment = folder.ListBlobsSegmentedAsync(
                    useFlatBlobListing: true,
                    blobListingDetails: BlobListingDetails.None,
                    maxResults: 1000,
                    currentToken: token,
                    options: null,
                    operationContext: null).Result;

                foreach (var blob in segment.Results)
                {
                    yield return blob.Uri.AbsolutePath;
                }

                token = segment.ContinuationToken;

            } while (token != null);
        }
        
        IEnumerable<string> MakeRelativePath(IEnumerable<string> files, Uri directory)
        {
            string folder = directory.AbsolutePath;

            var relativePath = files.Select(f => f.Substring(folder.Length + 1)).ToList();

            return relativePath;
        }
    }
}
