using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PdfTextReader;
using PdfTextReader.Azure.Blob;
using PdfTextReader.Azure;

namespace ParserFunctions
{
    public class AzureFS : IVirtualFS
    {
        AzureBlobFileSystem _inputFS = new AzureBlobFileSystem();
        AzureBlobFileSystem _outputFS = new AzureBlobFileSystem();

        public AzureFS(string inputConnectionString, string outputConnectionString)
        {
            if (String.IsNullOrEmpty(inputConnectionString))
                throw new ArgumentNullException(nameof(inputConnectionString));

            if (String.IsNullOrEmpty(outputConnectionString))
                throw new ArgumentNullException(nameof(outputConnectionString));

            _inputFS.AddStorageAccount("input", inputConnectionString);
            _outputFS.AddStorageAccount("output", outputConnectionString);
        }
        
        public Stream OpenReader(string filename) => _inputFS.GetFile(filename).GetStreamReader();
        
        public Stream OpenWriter(string filename) => _outputFS.GetFile(filename).GetStreamWriter();

        public IAzureBlobFolder GetFolder(string name)
        {
            return _inputFS.GetFolder(name);
        }
    }
}
