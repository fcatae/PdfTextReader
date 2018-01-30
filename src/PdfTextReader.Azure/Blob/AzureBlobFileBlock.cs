using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PdfTextReader.Azure.Blob
{
    public class AzureBlobFileBlock : AzureBlobRef
    {
        CloudBlockBlob _blob;

        public AzureBlobFileBlock(AzureBlobRef parent, string name, CloudBlockBlob blob) : base(parent, name, blob.Uri)
        {
            _blob = blob;
        }

        public Stream GetStreamWriter() => _blob.OpenWriteAsync().Result;

        public Stream GetStreamReader() => _blob.OpenReadAsync().Result;
    }
}
