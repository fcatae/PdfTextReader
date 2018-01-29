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

        public IEnumerable<AzureBlobRef> EnumerateFiles(string folder)
        {
            throw new NotImplementedException();

            if (String.IsNullOrEmpty(folder))
                throw new ArgumentNullException(nameof(folder));

            //var directory = _container.GetDirectoryReference(folder);

            //var files = EnumerateFilesInternal(directory);

            //return files;
        }

        public virtual AzureBlobFolder GetFolder(string folder)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<AzureBlobRef> EnumItems()
        {
            throw new NotImplementedException();
        }
    }
}
