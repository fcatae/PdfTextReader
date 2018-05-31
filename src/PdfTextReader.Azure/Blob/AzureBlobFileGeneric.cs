using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PdfTextReader.Azure.Blob
{
    class AzureBlobFileGeneric : AzureBlobRef, IAzureBlobFile
    {
        public AzureBlobFileGeneric(IAzureBlob parent, string name, Uri uri) : base(parent, name, uri)
        {            
        }

        public string Extension
        {
            get
            {
                var idx = Name.LastIndexOf('.');
                if (idx == -1)
                    return string.Empty;

                return Name.Substring(idx + 1);
            }
        }

        string IAzureBlobFile.Uri => Uri.ToString();

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public Stream GetStreamReader()
        {
            throw new NotImplementedException();
        }

        public Stream GetStreamWriter()
        {
            throw new NotImplementedException();
        }
    }
}
