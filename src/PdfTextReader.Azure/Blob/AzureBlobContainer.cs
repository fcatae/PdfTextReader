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
        
        public AzureBlobContainer(IAzureBlob parent, string name, CloudBlobContainer container) : base(parent, name, container.Uri)
        {
            _container = container;
        }

        protected override AzureBlobFolder GetChildFolder(string name)
        {
            var folder = _container.GetDirectoryReference(name);
            
            return new AzureBlobFolder(this, name, folder);
        }

        protected override AzureBlobFileBlock GetChildFile(string name)
        {
            if (_container == null)
                throw new InvalidOperationException();

            var blob = _container.GetBlockBlobReference(name);

            return new AzureBlobFileBlock(this, name, blob);
        }

        protected override BlobResultSegment ListBlobs(BlobContinuationToken token)
        {
            return _container.ListBlobsSegmentedAsync(
                    prefix: "",
                    useFlatBlobListing: false,
                    blobListingDetails: BlobListingDetails.None,
                    maxResults: 1000,
                    currentToken: token,
                    options: null,
                    operationContext: null).Result;
        }

        public override bool Exists()
        {
            return _container.ExistsAsync().Result;
        }
    }
}
