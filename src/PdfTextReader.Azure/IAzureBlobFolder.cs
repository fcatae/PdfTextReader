using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Azure
{
    public interface IAzureBlobFolder : IAzureBlob
    {
        IAzureBlobFolder GetFolder(string name);
        IAzureBlobFile GetFile(string name);
        IEnumerable<IAzureBlob> EnumItems();
        bool Exists();
    }
}
