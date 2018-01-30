using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PdfTextReader.Azure.Blob
{
    public abstract class AzureBlobRef
    {
        protected AzureBlobRef(string rootname)
        {
            if (String.IsNullOrEmpty(rootname))
                throw new ArgumentNullException(nameof(rootname));

            Path = rootname;
            Name = rootname;
        }

        public AzureBlobRef(AzureBlobRef parent, string name, Uri uri)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            EnsureValidName(name);
            
            Path = $"{parent.Path}/{name}";
            Name = name;
            Uri = uri;
        }

        public readonly string Name;
        public readonly string Path;
        public readonly Uri Uri;

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
