using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Azure.DevNul
{
    class DevNulFS : IAzureBlobFolder
    {
        const string PROTOCOL = "nul:/";

        public DevNulFS() : this(PROTOCOL)
        {
        }

        public DevNulFS(string protocol)
        {
            this.Path = protocol;
            this.Name = protocol;
        }

        public string Name { get; private set; }
        public string Path { get; private set; }

        public IEnumerable<IAzureBlob> EnumItems()
        {
            return new IAzureBlob[0];
        }

        public bool Exists()
        {
            return true;
        }

        public IAzureBlobFile GetFile(string path)
        {
            string name = GetName(path);

            if (name == null)
                throw new System.IO.FileNotFoundException(path);

            return new DevNulFile(this.Path, name);
        }

        public IAzureBlobFolder GetFolder(string path)
        {
            string name = GetName(path);

            if (name == null)
                throw new System.IO.DirectoryNotFoundException(path);

            return new DevNulFolder(this.Path, name);
        }

        string GetName(string path)
        {
            if (path == this.Path)
                return path;

            if (!path.StartsWith(this.Path))
                return null;

            return path.Substring(this.Path.Length + 1);
        }
    }
}
