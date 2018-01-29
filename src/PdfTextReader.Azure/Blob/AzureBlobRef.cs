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

        public AzureBlobRef(AzureBlobRef parent, string name)
        {
            Path = $"{parent.Path}/{name}";
            Name = name;
        }

        public readonly string Name;
        public readonly string Path;

        void EnsureValidName(string name)
        {
            if (name.Contains("/") || name.Contains("\\"))
                throw new ArgumentException("'Name' contains invalid characters");
        }
    }
}
