using PdfTextReader;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ParserRun
{
    class AzureFS : IVirtualFS
    {
        AzureBlob _blob;

        public AzureFS(string connectionString, string containerName)
        {
            _blob = new AzureBlob(connectionString, containerName);
        }

        public Stream OpenReader(string filename) => _blob.GetStreamReader(filename);

        public Stream OpenWriter(string filename) => _blob.GetStreamWriter(filename);
    }
}
