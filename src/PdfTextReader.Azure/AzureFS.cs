using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PdfTextReader;
using PdfTextReader.Azure.Blob;

namespace ParserFunctions
{
    class AzureFS : IVirtualFS
    {
        const string PROTOCOL = "wasb://";

        AzureBlobFS _blobFS;
        AzureBlobContainer _input;
        AzureBlobContainer _output;

        public AzureFS(AzureBlobContainer input, AzureBlobContainer output)
        {
            _blobFS = new AzureBlobFS();
            _input = input;
            _output = output;
        }

        public void AddStorageAccount(string name, AzureBlobAccount azureBlobAccount)
        {
            //_blobFS.Mount(name, azureBlobAccount);
        }

        public Stream OpenReader(string filename) => throw new NotImplementedException(); // _input.GetStreamReader(filename);

        public Stream OpenWriter(string filename) => throw new NotImplementedException(); //  _output.GetStreamWriter(filename);
    }
}
