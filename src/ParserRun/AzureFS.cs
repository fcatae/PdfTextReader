using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PdfTextReader;
using PdfTextReader.Azure.Blob;
using PdfTextReader.Azure;

namespace ParserRun
{
    public class AzureFS : IVirtualFS
    {
        AzureBlobFileSystem _blobFS = new AzureBlobFileSystem();
        
        public void AddStorageAccount(string name, string connectionString)
        {
            _blobFS.AddStorageAccount(name, connectionString);
        }
        
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
