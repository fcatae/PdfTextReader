using PdfTextReader;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ParserFunctions
{
    class AzureFS : IVirtualFS
    {
        AzureBlob _input;
        AzureBlob _output;

        public AzureFS(AzureBlob input, AzureBlob output)
        {
            _input = input;
            _output = output;
        }

        public Stream OpenReader(string filename) => _input.GetStreamReader(filename);

        public Stream OpenWriter(string filename) => _output.GetStreamWriter(filename);
    }
}
