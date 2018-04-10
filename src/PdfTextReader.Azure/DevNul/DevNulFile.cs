using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PdfTextReader.Azure.DevNul
{
    class DevNulFile : IAzureBlobFile
    {
        public DevNulFile(string path, string name)
        {
            this.Path = path;
            this.Name = name;
        }

        public string Name { get; private set; }
        public string Path { get; private set; }

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

        public string Uri => null;

        Stream AlwaysCreateNewStream() => new MemoryStream();

        public Stream GetStreamReader()
        {
            return AlwaysCreateNewStream();
        }

        public Stream GetStreamWriter()
        {
            return AlwaysCreateNewStream();
        }
    }
}
