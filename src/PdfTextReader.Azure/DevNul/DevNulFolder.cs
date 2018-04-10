using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Azure.DevNul
{
    class DevNulFolder : IAzureBlobFolder
    {
        public DevNulFolder(string path, string name)
        {
            this.Path = path;
            this.Name = name;
        }

        public string Name { get; private set; }
        public string Path { get; private set; }

        public string Extension { get{ return null;}  }

        public Uri Uri { get; }

        public IEnumerable<IAzureBlob> EnumItems()
        {
            return new IAzureBlob[0];
        }

        public bool Exists()
        {
            return true;
        }

        public IAzureBlobFile GetFile(string name)
        {
            return new DevNulFile(this.Path, name);
        }

        public IAzureBlobFolder GetFolder(string name)
        {
            return new DevNulFolder(this.Path, name);
        }
    }
}
