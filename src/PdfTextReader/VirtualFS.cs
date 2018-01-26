using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PdfTextReader.Base
{
    class VirtualFS
    {
        public static VirtualFS Storage = new VirtualFS();

        public Stream OpenReader(string filename)
        {
            return new FileStream(filename, FileMode.Open);
        }

        public void OpenWriter(string filename)
        {
        }
    }
}
