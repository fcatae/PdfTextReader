using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Azure.Blob
{
    public class AzureBlobFolder : AzureBlobRef
    {
        CloudBlobDirectory _folder;
        Uri _folderUri;

        protected AzureBlobFolder(string storageAccountAlia) : base(storageAccountAlia)
        {
            _folder = null;
        }

        protected AzureBlobFolder(AzureBlobRef parent, string containerName) : base(parent, containerName)
        {
            _folder = null;
        }

        public AzureBlobFolder(AzureBlobRef parent, string name, CloudBlobDirectory folder) : base(parent, name)
        {
            if (folder == null)
                throw new ArgumentNullException(nameof(folder));

            _folder = folder;
            _folderUri = folder.Uri;
        }
        
        public virtual AzureBlobFolder GetFolder(string name)
        {
            if (_folder == null)
                throw new InvalidOperationException();

            var blobDirectory = _folder.GetDirectoryReference(name);

            return new AzureBlobFolder(this, name, blobDirectory);
        }

        public virtual AzureBlobFile GetFile(string name)
        {
            if (_folder == null)
                throw new InvalidOperationException();

            // var blobDirectory = _folder.GetDirectoryReference(name);

            return new AzureBlobFile(this, name);
        }

        public virtual IEnumerable<AzureBlobRef> EnumItems()
        {
            BlobContinuationToken token = null;

            do
            {
                var segment = _folder.ListBlobsSegmentedAsync(
                    useFlatBlobListing: false,
                    blobListingDetails: BlobListingDetails.None,
                    maxResults: 1000,
                    currentToken: token,
                    options: null,
                    operationContext: null).Result;

                foreach (var blob in segment.Results)
                {
                    string name = MakeRelativePath(blob.Uri);

                    if (blob is CloudBlobDirectory)
                    {
                        yield return new AzureBlobFolder(this, name, (CloudBlobDirectory)blob);
                    }
                    else if (blob is CloudBlob)
                    {
                        yield return new AzureBlobFile(this, name);
                    }
                }

                token = segment.ContinuationToken;

            } while (token != null);
        }

        string MakeRelativePath(Uri childFolderUri)
        {
            var relativeUri = childFolderUri.MakeRelativeUri(_folderUri);

            return relativeUri.AbsolutePath;

            //string fullpath = childFolderUri.AbsolutePath;
            //string basepath = _folderUri.AbsolutePath;

            //return fullpath.Substring(basepath.Length);
        }
    }
}
