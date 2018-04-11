using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace PdfTextReader.Base
{
    partial class VirtualFS : IVirtualFS
    {   
        public static Stream OpenRead(string filename)
        {
            return g_vfs.OpenReader(filename);
        }
        public static Stream OpenWrite(string filename)
        {
            return g_vfs.OpenWriter(filename);
        }

        [DebuggerHidden]
        public static void ConfigureFileSystem(IVirtualFS virtualFS)
        {
            if (virtualFS == null)
                throw new ArgumentNullException(nameof(IVirtualFS));

            g_vfs = virtualFS;
        }

        // iText.Kernel.Pdf
        public static iText.Kernel.Pdf.PdfReader OpenPdfReader(string filename)
        {
            return new iText.Kernel.Pdf.PdfReader(OpenRead(filename));
        }

        public static iText.Kernel.Pdf.PdfWriter OpenPdfWriter(string filename)
        {
            return new iText.Kernel.Pdf.PdfWriter(OpenWrite(filename));
        }
        
        public static StreamWriter OpenStreamWriter(string filename)
        {
            return new StreamWriter(OpenWrite(filename));
        }
        
    }
}
