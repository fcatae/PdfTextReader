using PdfTextReader.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PdfTextReader.Base
{
    public partial class VirtualFS : IVirtualFS
    {
        static IVirtualFS g_vfs = new VirtualFS();

        public Stream OpenReader(string filename)
        {
            System.Diagnostics.Debug.WriteLine($"READ: {filename}");
            return new FileStream(filename, FileMode.Open, FileAccess.Read);
        }

        public Stream OpenWriter(string filename)
        {
            System.Diagnostics.Debug.WriteLine($"WRITE: {filename}");
            return new FileStream(filename, FileMode.Create);
        }        
    }
}
