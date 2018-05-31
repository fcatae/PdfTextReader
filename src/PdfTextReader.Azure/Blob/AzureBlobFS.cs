using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PdfTextReader.Azure.Blob
{
    class AzureBlobFS : IAzureBlobFolder
    {
        const string PROTOCOL = "wasb:/"; // just one slash
        readonly char[] PATH_SEPARATORS = new[] { '/', '\\' };
        public string Name { get; }
        public string Path { get; }

        public AzureBlobFS() : this(PROTOCOL)
        {
        }
        public AzureBlobFS(string protocol)
        {
            if (protocol == null)
                throw new InvalidOperationException();

            Name = protocol;
            Path = protocol;
        }

        Dictionary<string, AzureBlobAccount> _storageAccounts = new Dictionary<string, AzureBlobAccount>();

        public void AddStorage(string path, string connectionString)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (String.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            string name = path.Trim(PATH_SEPARATORS);

            if (name == "")
                throw new ArgumentException(nameof(name));

            CheckValidMountPath(name);

            AzureBlobAccount azureBlobAccount = new AzureBlobAccount(this, name, connectionString);

            string key = name + '/';

            _storageAccounts.Add(key, azureBlobAccount);
        }

        public IAzureBlobFolder GetFolder(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (path == null)
                return this;

            var account = FindStorageAccount(path);

            if (account == null)
                throw new System.IO.DirectoryNotFoundException($"There is no storage account configured for '{path}'");

            string blobpath = GetBlobPath(account, path);

            if (blobpath == "")
                return account;

            return account.GetFolder(blobpath);
        }

        public IAzureBlobFile GetFile(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if ( path == "" )
                throw new System.IO.FileNotFoundException($"'{path}' is empty");

            var account = FindStorageAccount(path);

            if (account == null)
                throw new System.IO.FileNotFoundException($"There is no storage account configured for '{path}'");

            string blobpath = GetBlobPath(account, path);

            if (blobpath == "")
                throw new System.IO.FileNotFoundException($"'{path}' is a storage account, not a file");

            return account.GetFile(blobpath);
        }

        AzureBlobAccount FindStorageAccount(string fullpath)
        {
            string fullpathKey = fullpath + '/';

            foreach (string subpath in _storageAccounts.Keys)
            {
                if(fullpathKey.StartsWith(subpath))
                {
                    return _storageAccounts[subpath];
                }
            }

            return null;
        }

        string GetBlobPath(AzureBlobAccount account, string path)
        {
            if (path == account.Name)
                return "";

            return path.Substring(account.Name.Length + 1);
        }

        public bool Exists()
        {
            return true;
        }

        public IEnumerable<IAzureBlob> EnumItems()
        {
            return _storageAccounts.Values;
        }

        [DebuggerHidden]
        void CheckValidMountPath(string path)
        {
            if (path.Contains(":") || path.Contains("?") || path.Contains("*"))
                throw new InvalidOperationException($"Path '{path}' contains invalid characters");

            foreach(var existingPath in _storageAccounts.Keys)
            {
                if (path == existingPath)
                    throw new InvalidOperationException($"Path '{path} already mounted in AzureBlobFS");

                if( existingPath.StartsWith(path) || path.StartsWith(existingPath))
                    throw new InvalidOperationException($"Path '{path}' conflicts with the existing path '{existingPath}' in AzureBlobFS");
            }
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }
    }
}
