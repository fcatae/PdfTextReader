using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PdfTextReader.Azure.Blob
{
    public abstract class AzureBlobRef
    {
        const string PROTOCOL = "wasb://";

        protected AzureBlobRef(string rootname)
        {
            EnsureValidName(rootname);

            Path = PROTOCOL + rootname;
            Name = rootname;
        }

        public AzureBlobRef(AzureBlobRef parent, string name)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            EnsureValidName(name);
            
            Path = $"{parent.Path}/{name}";
            Name = name;
        }

        public readonly string Name;
        public readonly string Path;

        [DebuggerHidden]
        void EnsureValidName(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (name.Contains("/") || name.Contains("\\"))
                throw new ArgumentException("'Name' contains invalid characters");
        }
    }
}
