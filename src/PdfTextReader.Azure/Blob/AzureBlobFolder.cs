using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PdfTextReader.Azure.Blob
{
    class AzureBlobFolder : AzureBlobRef, IAzureBlobFolder
    {
        readonly char[] PATH_SEPARATORS = new[] {'/', '\\'};

        CloudBlobDirectory _folder;

        protected AzureBlobFolder(IAzureBlob parent, string containerName, Uri folderUri) : base(parent, containerName, folderUri)
        {
            if (folderUri == null)
                throw new ArgumentNullException(nameof(folderUri));

            _folder = null;
        }

        public AzureBlobFolder(IAzureBlob parent, string name, CloudBlobDirectory folder) : base(parent, name, folder.Uri)
        {
            if (folder == null)
                throw new ArgumentNullException(nameof(folder));

            _folder = folder;
        }

        public IAzureBlobFolder GetFolder(string name)
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
            string[] components = SplitFolderComponents(name);
            
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

        public IAzureBlobFile GetFile(string name)
        {
            CheckValidFilePath(name);

            AzureBlobFolder folder = this;

            string[] components = SplitFileComponents(name);

            string filename = components[0];

            // Get folder
            if (components.Length > 1)
            {
                string parentDirectory = components[0];
                folder = GetChildFolderRecursive(parentDirectory);

                filename = components[1];
            }

            return folder.GetChildFile(filename);
        }

        protected virtual AzureBlobFileBlock GetChildFile(string name)
        {
            if (_folder == null)
                throw new InvalidOperationException();

            var blob = _folder.GetBlockBlobReference(name);

            // throw exception if it does not exist
            CheckExists(blob);

            return new AzureBlobFileBlock(this, name, blob);
        }

        public virtual IEnumerable<IAzureBlob> EnumItems()
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
                        yield return new AzureBlobFileGeneric(this, name, blob.Uri);
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
        
        string MakeRelativePath(Uri childFolderUri)
        {
            string fullpath = childFolderUri.AbsolutePath;
            string basepath = Uri.AbsolutePath;

            string relativePath = fullpath.Substring(basepath.Length);

            return relativePath.Trim(PATH_SEPARATORS);
        }
        
        void CheckValidFileName(string name)
        {
            if (name == null || name == "")
                throw new ArgumentNullException();

            if (name == "..")
                throw new System.IO.DirectoryNotFoundException($"Accessing parent folder ../ is not allowed");

            if (name.Contains("/") || name.Contains("\\") || name.Contains(":"))
                throw new InvalidOperationException($"Invalid characters in path name");
        }

        void CheckValidFilePath(string path)
        {
            if (path == null || path == "")
                throw new ArgumentNullException();

            if (path.StartsWith("/"))
                throw new System.IO.DirectoryNotFoundException($"Cannot access root folder");

            if (path.EndsWith("/"))
                throw new System.IO.FileNotFoundException($"'{path}' ends with '/'");
        }

        string[] SplitFolderComponents(string path)
        {
            int idxSeparator = path.IndexOfAny(PATH_SEPARATORS);

            return SplitComponents(path, idxSeparator);
        }

        string[] SplitFileComponents(string path)
        {
            int idxSeparator = path.LastIndexOfAny(PATH_SEPARATORS);

            return SplitComponents(path, idxSeparator);
        }

        string[] SplitComponents(string path, int idxSeparator)
        {
            if (idxSeparator < 0)
                return new string[] { path };

            string folder = path.Substring(0, idxSeparator);
            string filename = path.Substring(idxSeparator + 1);

            return new string[] { folder, filename };
        }
    }
}
