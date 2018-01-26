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

        // System.IO.TextStream
        public static StreamReader OpenStreamReader(string filename)
        {
            return new StreamReader(OpenRead(filename));
        }
        
        public static StreamWriter OpenStreamWriter(string filename)
        {
            return new StreamWriter(OpenWrite(filename));
        }

        // XML
        public static System.Xml.XmlWriter OpenXmlWriter(string filename, System.Xml.XmlWriterSettings settings)
        {
            return System.Xml.XmlWriter.Create(OpenWrite(filename), settings);
        }


        // Internal validators

        // System.IO: Directory, DirectoryInfo
        public static string GetDirectoryName(string folder)
        {
            return new DirectoryInfo(folder).Name;
        }

        public static string GetDirectoryCreateDirectory(string folder)
        {
            return Directory.CreateDirectory(folder).FullName;
        }
        
        public static IEnumerable<VFileInfo> DirectoryInfoEnumerateFiles(string folder, string pattern)
        {
            var directory = new DirectoryInfo(folder);

            var enumFiles = directory.EnumerateFiles("*.xml");

            return enumFiles.Select(f => new VFileInfo(f));
        }

        public static void FileWriteAllText(string filename, string text)
        {
            File.WriteAllText(filename, text);
        }

        public static System.Xml.Linq.XDocument XDocumentLoad(string filename)
        {
            return System.Xml.Linq.XDocument.Load(filename);
        }

        public class VFileInfo
        {
            FileInfo _f;

            public VFileInfo(FileInfo f)
            {
                _f = f;
            }

            public string Name => _f.Name;
            public string FullName => _f.FullName;
            public void CopyTo(string dest)
            {
                _f.CopyTo(dest);
            }
        }
    }
}
