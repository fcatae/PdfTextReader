using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PdfTextReader.Base
{
    class VirtualFS
    {
        public static VirtualFS Storage = new VirtualFS();

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
        
        public static iText.Kernel.Pdf.PdfReader OpenReader(string filename)
        {
            return new iText.Kernel.Pdf.PdfReader(filename);
        }
        public static iText.Kernel.Pdf.PdfWriter OpenWriter(string filename)
        {
            return new iText.Kernel.Pdf.PdfWriter(filename);
        }

    }
}
