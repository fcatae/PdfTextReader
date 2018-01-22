using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Validator
{
    class FileList
    {
        private readonly string _filePattern;

        public FileList(string pattern)
        {
            _filePattern = pattern;
        }

        public static string CreateOutputFolder(string outputFolder, string basename)
        {
            string outpath = Path.Combine(outputFolder, basename);

            Directory.CreateDirectory(outpath);

            return outpath;
        }

        public File[] EnumFiles(string foldername)
        {
            return RecursiveEnumFiles(foldername).ToArray();
        }

        IEnumerable<File> RecursiveEnumFiles(string foldername)
        {
            var directory = new DirectoryInfo(foldername);

            foreach (var file in directory.EnumerateFiles(_filePattern))
            {
                string filename = file.Name;
                string basename = Path.GetFileNameWithoutExtension(filename);
                
                yield return new File(foldername, basename);
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
