using PdfTextReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ParserFrontend
{
    public class WebVirtualFS : IVirtualFS2
    {
        const string FILEFOLDERWWW = "wwwroot/files";

        public Stream OpenReader(string virtualfile)
        {
            string filename = GetLocalFilename(virtualfile);
            
            return new FileStream(filename, FileMode.Open, FileAccess.Read);
        }

        public Stream OpenWriter(string virtualfile)
        {
            string filename = GetLocalFilename(virtualfile);
            
            string folderName = Path.GetDirectoryName(filename);
            if (!Directory.Exists(folderName))
            {
                if (Path.IsPathRooted(folderName))
                {
                    throw new NotImplementedException();
                }
                else
                {
                    DirectoryInfo directory = new DirectoryInfo(".");
                    directory.CreateSubdirectory(folderName);
                }
            }

            return new FileStream(filename, FileMode.Create);
        }

        public void Delete(string virtualfile)
        {
            string filename = GetLocalFilename(virtualfile) + ".pdf";

            File.Delete(filename);
        }

        public void DeleteFolder(string virtualfile)
        {
            string foldername = GetLocalFilename(virtualfile);

            Directory.Delete(foldername, true);
        }

        //public string[] ListFiles(string pattern)
        //{
        //    DirectoryInfo directory = new DirectoryInfo("wwwroot/files/input");

        //    var files = directory.EnumerateFiles(pattern).Select(fi => fi.Name);

        //    return files.ToArray();
        //}

        public string[] ListFileExtension(string extension)
        {
            string pattern = "*" + extension;

            DirectoryInfo directory = new DirectoryInfo("wwwroot/files/input");

            var files = directory.EnumerateFiles(pattern).Select(fi => fi.Name);

            return files.ToArray();
        }

        public string[] ListFolderContent(string folder)
        {
            string pattern = "*.*";

            DirectoryInfo directory = new DirectoryInfo($"wwwroot/files/{folder}");

            var files = directory.EnumerateFiles(pattern).Select(fi => $"{folder}/{fi.Name}");

            return files.ToArray();
        }

        string GetLocalFilename(string virtualfile)
        {
            if (virtualfile.Contains(".."))
                throw new FileNotFoundException("Invalid path");

            return $"{FILEFOLDERWWW}/{virtualfile}";
        }
    }
}
