using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Azure.Blob
{
    public abstract class AzureBlobRef
    {
        const string PROTOCOL = "wasb://";

        protected AzureBlobRef(string rootname)
        {
            Path = PROTOCOL + rootname;
            Name = rootname;
        }

        public AzureBlobRef(AzureBlobRef parent, string filename)
        {
            Path = $"{parent.Path}/{filename}";
            Name = filename;
        }

        public readonly string Name;
        public readonly string Path;
    }
}
