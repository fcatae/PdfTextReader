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
        
        public AzureBlobContainer(AzureBlobAccount parent, string name, CloudBlobContainer container) : base(parent, name)
        {
            _container = container;
        }
        
        public IEnumerable<AzureBlobRef> EnumerateFiles()
        {
            var files = EnumerateFilesInternal();

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
                    string name = blob.Uri.AbsolutePath;

                    if ( blob is CloudBlobDirectory )
                    {                        
                        yield return new AzureBlobFolder(this, name, (CloudBlobDirectory)blob);
                    }
                    else
                    {
                        yield return new AzureBlobFile(this, name);
                    }
                }

                token = segment.ContinuationToken;

            } while (token != null);
        }
    }
}
