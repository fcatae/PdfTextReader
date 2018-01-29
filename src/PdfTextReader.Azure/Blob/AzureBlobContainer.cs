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

namespace PdfTextReader.Azure.Blob
{
    class AzureBlobContainer : AzureBlobFolder
    {
        private readonly CloudBlobContainer _container;
        private readonly string _containerPath;

        public string UriPath => _containerPath;

        public AzureBlobContainer(string connectionString, string containerName) : base(containerName)
        {
            _container = GetContainer(connectionString, containerName);
            _containerPath = _container.Uri.AbsolutePath;
        }

        private CloudBlobContainer GetContainer(string conn, string containerName)
        {
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
            var blockBlob = _container.GetBlockBlobReference(filename);

            await _container.CreateIfNotExistsAsync();

            return await blockBlob.OpenWriteAsync();
        }
        
        public Task<Stream> GetStreamReaderAsync(string filename)
        {
            var blockBlob = _container.GetBlockBlobReference(filename);

            return blockBlob.OpenReadAsync();
        }

        public IEnumerable<AzureBlobRef> EnumerateFiles()
        {
            var files = EnumerateFilesInternal();

            return files;
        }

        public IEnumerable<AzureBlobRef> EnumerateFiles(string folder)
        {
            if ( String.IsNullOrEmpty(folder) )
                throw new ArgumentNullException(nameof(folder));

            var directory = _container.GetDirectoryReference(folder);

            var files = EnumerateFilesInternal(directory);

            return files;
        }

        IEnumerable<AzureBlobRef> EnumerateFilesInternal()
        {
            BlobContinuationToken token = null;

            do
            {
                var segment = _container.ListBlobsSegmentedAsync(
                    prefix: "",
                    useFlatBlobListing: false,
                    blobListingDetails: BlobListingDetails.None,
                    maxResults: 1000,
                    currentToken: token,
                    options: null,
                    operationContext: null).Result;

                foreach (var blob in segment.Results)
                {
                    string blobPath = blob.Uri.AbsolutePath;

                    if ( blob is CloudBlobDirectory )
                    {                        
                        yield return new AzureBlobFolder(this, blobPath);
                    }
                    else
                    {
                        yield return new AzureBlobFile(this, blobPath);
                    }
                }

                token = segment.ContinuationToken;

            } while (token != null);
        }

        IEnumerable<AzureBlobRef> EnumerateFilesInternal(CloudBlobDirectory folder)
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
                    string blobPath = blob.Uri.AbsolutePath;

                    if (blob is CloudBlobDirectory)
                    {
                        yield return new AzureBlobFolder(this, blobPath);
                    }
                    else
                    {
                        yield return new AzureBlobFile(this, blobPath);
                    }
                }

                token = segment.ContinuationToken;

            } while (token != null);
        }
    }
}
