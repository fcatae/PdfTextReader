using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Azure.Blob
{
    public class AzureBlobFolder : AzureBlobRef
    {
        protected AzureBlobFolder(string filename) : base(filename)
        {
        }

        public AzureBlobFolder(AzureBlobRef parent, string filename) : base(parent, filename)
        {
        }

        public virtual AzureBlobFolder GetFolder(string folder)
        {
            throw new NotImplementedException();
        }

        public virtual AzureBlobRef EnumFiles()
        {
            throw new NotImplementedException();
        }
    }
}
