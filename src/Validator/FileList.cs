using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Validator
{
    class FileList
    {
        private readonly string _foldername;

        public FileList(string foldername)
        {
            _foldername = foldername;
        }
        public string GetFolderName()
        {
            var directory = new DirectoryInfo(_foldername);

            return directory.FullName;
        }

        public File[] RecursiveEnumFiles()
        {
            return RecursiveEnumFiles(_foldername).ToArray();
        }

        IEnumerable<File> RecursiveEnumFiles(string foldername)
        {
            var directory = new DirectoryInfo(foldername);

            string filePattern = "*.pdf";

            foreach (var file in directory.EnumerateFiles(filePattern))
            {
                string filename = file.Name;

                if (!filename.ToLower().EndsWith("-output.pdf"))
                {
                    yield return new File(foldername, filename);
                }
            }

            foreach (var childDirectory in directory.EnumerateDirectories())
            {
                string childFolder = childDirectory.FullName;

                foreach (var childFilename in RecursiveEnumFiles(childFolder))
                {
                    yield return childFilename;
                }
            }
        }
    }
}
