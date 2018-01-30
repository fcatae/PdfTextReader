using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public AzureBlobFolder GetFolder(string name)
        {
            var folder = GetChildFolderRecursive(name);

            // throw exception if it does not exist
            CheckExists(folder);
            
            return folder;
        }

        public virtual bool Exists()
        {
            if (_folder == null)
                throw new InvalidOperationException();

            var item = _folder.ListBlobsSegmentedAsync(
                useFlatBlobListing: false,
                blobListingDetails: BlobListingDetails.None,
                maxResults: 1,
                currentToken: null,
                options: null,
                operationContext: null).Result;

            return (item.Results.FirstOrDefault() != null);
        }

        AzureBlobFolder GetChildFolderRecursive(string name)
        {
            string[] components = name.Split(new[] { '/' }, 1, StringSplitOptions.RemoveEmptyEntries);
            
            // parent
            string parentDirectory = components[0];

            var current = GetChildFolder(parentDirectory);
            
            if (components.Length == 1)
            {
                return current;
            }

            // child - recursive
            string childDirectory = components[1];

            return current.GetChildFolderRecursive(childDirectory);
        }

        protected virtual AzureBlobFolder GetChildFolder(string name)
        {
            if (_folder == null)
                throw new InvalidOperationException();

            CheckValidFileName(name);

            var blobDirectory = _folder.GetDirectoryReference(name);

            return new AzureBlobFolder(this, name, blobDirectory);
        }

        public virtual AzureBlobFileBlock GetFile(string name)
        {
            if (_folder == null)
                throw new InvalidOperationException();

            var blob = _folder.GetBlockBlobReference(name);

            // throw exception if it does not exist
            CheckExists(blob);
            
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

        [DebuggerHidden]
        void CheckExists(AzureBlobFolder folder)
        {
            if (folder.Exists() == false)
                throw new System.IO.DirectoryNotFoundException($"Folder '{folder.Uri.AbsoluteUri}' does not exist");
        }

        [DebuggerHidden]
        void CheckExists(CloudBlockBlob blob)
        {
            if(blob.ExistsAsync().Result == false)
                throw new System.IO.FileNotFoundException($"File '{blob.Uri.AbsoluteUri}' does not exist");
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
        
        string MakeRelativePath(Uri childFolderUri)
        {
            string fullpath = childFolderUri.AbsolutePath;
            string basepath = _folderUri.AbsolutePath;

            string relativePath = fullpath.Substring(basepath.Length);

            return relativePath.Trim('/');
        }
        
        void CheckValidFileName(string name)
        {
            if (name == "..")
                throw new System.IO.DirectoryNotFoundException($"Accessing parent folder ../ is not allowed");

            if (name.Contains("/") || name.Contains(":"))
                throw new InvalidOperationException($"Invalid characters in path name");
        }
    }
}
