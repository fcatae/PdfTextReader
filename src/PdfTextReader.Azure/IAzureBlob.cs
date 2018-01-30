using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Azure
{
    public interface IAzureBlob
    {
        string Name { get; }
        string Path { get; }
    }
}
