using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PdfTextReader;
using PdfTextReader.Azure.Blob;

namespace PdfTextReader.Azure
{
    public class AzureFS : IVirtualFS
    {
        AzureBlobFS _blobFS = new AzureBlobFS();
        string _workingFolder;
        
        public void AddStorageAccount(string name, string connectionString)
        {
            _blobFS.AddStorage(name, connectionString);
        }

        public void SetWorkingFolder(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            _workingFolder = path;
        }

        public string GetWorkingFolder() => _workingFolder;

        public Stream OpenReader(string filename) => _blobFS.GetFile(filename).GetStreamReader();
        
        public Stream OpenWriter(string filename) => _blobFS.GetFile(filename).GetStreamWriter();

        public IAzureBlobFolder GetFolder(string name)
        {
            return _blobFS.GetFolder(name);
        }

        public IAzureBlobFile GetFile(string name)
        {
            return _blobFS.GetFile(name);
        }

        public IEnumerable<IAzureBlob> EnumItems()
        {
            return _blobFS.EnumItems();
        }        
    }
}
