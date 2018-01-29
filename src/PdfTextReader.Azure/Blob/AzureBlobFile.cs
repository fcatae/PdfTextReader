using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Azure.Blob
{
    class AzureBlobFile : AzureBlobRef
    {
        public AzureBlobFile(AzureBlobRef parent, string filename) : base(parent, filename)
        {
        }
    }
}
