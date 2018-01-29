using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
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

        protected AzureBlobFolder(AzureBlobRef parent, string containerName, Uri folderUri) : base(parent, containerName)
        {
            if (folderUri == null)
                throw new ArgumentNullException(nameof(folderUri));

            _folder = null;
            _folderUri = folderUri;
        }

        public AzureBlobFolder(AzureBlobRef parent, string name, CloudBlobDirectory folder) : base(parent, name)
        {
            if (folder == null)
                throw new ArgumentNullException(nameof(folder));

            _folder = folder;
            _folderUri = folder.Uri;
        }
        
        public virtual AzureBlobFolder GetFolderReference(string name)
        {
            if (_folder == null)
                throw new InvalidOperationException();

            var blobDirectory = _folder.GetDirectoryReference(name);

            CheckExists(blobDirectory);

            return new AzureBlobFolder(this, name, blobDirectory);
        }

        public virtual AzureBlobFileBlock GetFile(string name)
        {
            if (_folder == null)
                throw new InvalidOperationException();

            var blob = _folder.GetBlockBlobReference(name);

            return new AzureBlobFileBlock(this, name, blob);
        }

        public virtual IEnumerable<AzureBlobRef> EnumItems()
        {
            BlobContinuationToken token = null;

            do
            {
                var segment = ListBlobs(token);

                foreach (var blob in segment.Results)
                {
                    string name = MakeRelativePath(blob.Uri);

                    if (blob is CloudBlobDirectory)
                    {
                        yield return new AzureBlobFolder(this, name, (CloudBlobDirectory)blob);
                    }
                    else if(blob is CloudBlockBlob)
                    {
                        yield return new AzureBlobFileBlock(this, name, (CloudBlockBlob)blob);
                    }
                    else
                    {
                        yield return new AzureBlobFileGeneric(this, name);
                    }
                }

                token = segment.ContinuationToken;

            } while (token != null);
        }

        void CheckExists(CloudBlobDirectory folder)
        {
            var item = folder.ListBlobsSegmentedAsync(
                    useFlatBlobListing: false,
                    blobListingDetails: BlobListingDetails.None,
                    maxResults: 1,
                    currentToken: null,
                    options: null,
                    operationContext: null).Result;

            if (item.Results.FirstOrDefault() == null)
                throw new System.IO.DirectoryNotFoundException($"Folder '{folder.Uri.AbsoluteUri}' does not exist");
        }

        protected virtual BlobResultSegment ListBlobs(BlobContinuationToken token)
        {
            return _folder.ListBlobsSegmentedAsync(
                    useFlatBlobListing: false,
                    blobListingDetails: BlobListingDetails.None,
                    maxResults: 1000,
                    currentToken: token,
                    options: null,
                    operationContext: null).Result;
        }

        protected IEnumerable<AzureBlobRef> EnumSegment(IEnumerable<IListBlobItem> segments)
        {

            foreach (var blob in segments)
            {
                string name = MakeRelativePath(blob.Uri);

                if (blob is CloudBlobDirectory)
                {
                    yield return new AzureBlobFolder(this, name, (CloudBlobDirectory)blob);
                }
                else
                {
                    yield return new AzureBlobFileGeneric(this, name);
                }
            }
        }
        
        string GetName(CloudBlobDirectory folder)
        {
            string name = folder.Prefix;

            return name.TrimEnd('/');
        }

        string MakeRelativePath(Uri childFolderUri)
        {
            string fullpath = childFolderUri.AbsolutePath;
            string basepath = _folderUri.AbsolutePath;

            string relativePath = fullpath.Substring(basepath.Length);

            return relativePath.Trim('/');
        }
    }
}
