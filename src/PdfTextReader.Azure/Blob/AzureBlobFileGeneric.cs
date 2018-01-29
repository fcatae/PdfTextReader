using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PdfTextReader.Azure.Blob
{
    public class AzureBlobFileGeneric : AzureBlobRef
    {
        public AzureBlobFileGeneric(AzureBlobRef parent, string name) : base(parent, name)
        {
        }
    }
}
