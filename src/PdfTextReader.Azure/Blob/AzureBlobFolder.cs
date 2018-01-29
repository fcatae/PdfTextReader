using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Azure.Blob
{
    class AzureBlobFolder : AzureBlobRef
    {
        protected AzureBlobFolder(string filename) : base(filename)
        {
        }

        public AzureBlobFolder(AzureBlobRef parent, string filename) : base(parent, filename)
        {
        }
    }
}
