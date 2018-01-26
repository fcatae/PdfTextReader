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
        public static iText.Kernel.Pdf.PdfReader OpenPdfReader(string filename)
        {
            return new iText.Kernel.Pdf.PdfReader(filename);
        }
        public static iText.Kernel.Pdf.PdfWriter OpenPdfWriter(string filename)
        {
            return new iText.Kernel.Pdf.PdfWriter(filename);
        }
        public static StreamReader OpenStreamReader(string filename)
        {
            return new StreamReader(filename);
        }
        
        public static StreamWriter OpenStreamWriter(string filename)
        {
            return new StreamWriter(filename);
        }

        public static System.Xml.XmlWriter OpenXmlWriter(string finalURL, System.Xml.XmlWriterSettings settings)
        {
            return System.Xml.XmlWriter.Create($"{finalURL}.xml", settings);
        }

        public static string GetDirectoryName(string folder)
        {
            return new DirectoryInfo(folder).Name;
        }

        public static string GetDirectoryCreateDirectory(string folder)
        {
            return Directory.CreateDirectory(folder).FullName;
        }
        public static void DirectoryCreateDirectory(string outpath)
        {
            Directory.CreateDirectory(outpath);
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
