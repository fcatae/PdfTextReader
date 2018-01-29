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
        AzureBlobContainer _input;
        AzureBlobContainer _output;

        public AzureFS(AzureBlobContainer input, AzureBlobContainer output)
        {
            _input = input;
            _output = output;
        }

        public Stream OpenReader(string filename) => throw new NotImplementedException(); // _input.GetStreamReader(filename);

        public Stream OpenWriter(string filename) => throw new NotImplementedException(); //  _output.GetStreamWriter(filename);
    }
}
