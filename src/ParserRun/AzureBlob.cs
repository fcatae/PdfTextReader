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

namespace ParserRun
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
            // container.CreateIfNotExists();

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
            return EnumerateFiles(folder, "");
        }

        public IEnumerable<string> EnumerateFiles(string folder, string prefix)
        {
            if (folder == null || prefix == null)
                throw new ArgumentNullException();

            string folderPath = GetFolderPath(folder);
            string path = folderPath + prefix;

            var files = EnumerateFilesInternal(path).ToList();

            return files;
        }

        IEnumerable<string> EnumerateFilesInternal(string prefix)
        {
            var container = GetContainer();

            BlobContinuationToken token = null;

            do
            {
                var segment = container.ListBlobsSegmentedAsync(
                    prefix,
                    useFlatBlobListing: true,
                    blobListingDetails: BlobListingDetails.None,
                    maxResults: 1000,
                    currentToken: token,
                    options: null,
                    operationContext: null).Result;

                foreach (var blob in segment.Results)
                {
                    yield return GetRelativePath(blob.Uri);
                }

                token = segment.ContinuationToken;

            } while (token != null);
        }

        string GetFolderPath(string folder)
        {
            return (folder.EndsWith("/")) ? folder : folder + "/";
        }

        string GetRelativePath(string folder, string path)
        {
            return path.Substring(folder.Length + 1);
        }

        string GetRelativePath(Uri uri)
        {
            string path = uri.AbsolutePath;
            
            // file: /container/...
            return path.Substring(_containerName.Length + 2);
        }        
    }
}
