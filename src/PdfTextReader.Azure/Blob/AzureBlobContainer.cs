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
    public class AzureBlobContainer : AzureBlobFolder
    {
        private readonly CloudBlobContainer _container;
        
        public AzureBlobContainer(AzureBlobAccount parent, string name, CloudBlobContainer container) : base(parent, name, container.Uri)
        {
            _container = container;
        }

        public override AzureBlobFolder GetFolderReference(string name)
        {
            var folder = _container.GetDirectoryReference(name);

            // check folder existance

            return new AzureBlobFolder(this, name, folder);
        }

        public override AzureBlobFileBlock GetFile(string name)
        {
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
    }
}
