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
        public static IVirtualFS Storage = new VirtualFS();

        public Stream OpenReader2(string filename)
        {
            System.Diagnostics.Debug.WriteLine($"READ: {filename}");
            return new FileStream(filename, FileMode.Open);
        }

        public Stream OpenWriter2(string filename)
        {
            System.Diagnostics.Debug.WriteLine($"WRITE: {filename}");
            return new FileStream(filename, FileMode.CreateNew);
        }        
    }
}
