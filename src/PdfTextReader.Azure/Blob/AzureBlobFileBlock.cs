using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PdfTextReader.Azure.Blob
{
    class AzureBlobFileBlock : AzureBlobRef, IAzureBlobFile
    {
        CloudBlockBlob _blob;

        public string Extension {
            get {
                var idx = Name.LastIndexOf('.');
                if (idx == -1)
                    return string.Empty;

                return Name.Substring(idx + 1);
            }
        }

        string IAzureBlobFile.Uri => Uri.ToString();

        public AzureBlobFileBlock(IAzureBlob parent, string name, CloudBlockBlob blob) : base(parent, name, blob.Uri)
        {
            _blob = blob;
        }

        public Stream GetStreamWriter() => _blob.OpenWriteAsync().Result;

        public Stream GetStreamReader() => _blob.OpenReadAsync().Result;
    }
}
