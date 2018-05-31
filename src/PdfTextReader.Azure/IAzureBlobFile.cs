using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PdfTextReader.Azure
{
    public interface IAzureBlobFile : IAzureBlob
    {
        void Delete();
        Stream GetStreamWriter();
        Stream GetStreamReader();
        string Extension { get; }
        string Uri { get; }
    }
}
