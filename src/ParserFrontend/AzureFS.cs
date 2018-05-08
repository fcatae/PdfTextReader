using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PdfTextReader;
using PdfTextReader.Azure.Blob;
using PdfTextReader.Azure;

namespace ParserFrontend
{
    public class AzureFS : IVirtualFS
    {
        AzureBlobFileSystem _azure = new AzureBlobFileSystem();
        AzureBlobFileSystem _inputFS;

        public AzureFS(string inputConnectionString)
        {
            _inputFS = _azure;

            if (String.IsNullOrEmpty(inputConnectionString))
                throw new ArgumentNullException(nameof(inputConnectionString));

            _inputFS.AddStorageAccount("azure", inputConnectionString);
            _inputFS.SetWorkingFolder("wasb://azure/web");
        }
        
        public Stream OpenReader(string filename) => _inputFS.GetFile(filename).GetStreamReader();
        
        public Stream OpenWriter(string filename) => _inputFS.GetFile(filename).GetStreamWriter();

        public IAzureBlobFolder GetFolder(string name)
        {
            return _inputFS.GetFolder(name);
        }
    }
}
