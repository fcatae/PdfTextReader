using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PdfTextReader;
using PdfTextReader.Azure.Blob;
using PdfTextReader.Azure;
using System.Linq;

namespace ParserFrontend
{
    public class AzureFS : IVirtualFS2
    {
        AzureBlobFileSystem _azure = new AzureBlobFileSystem();
        AzureBlobFileSystem _inputFS;

        public AzureFS(string inputConnectionString)
        {
            _inputFS = _azure;

            if (String.IsNullOrEmpty(inputConnectionString))
                throw new ArgumentNullException(nameof(inputConnectionString));

            _inputFS.AddStorageAccount("azure", inputConnectionString);
            _inputFS.SetWorkingFolder("wasb://azure/web");
        }

        public AzureFS(string inputConnectionString, string container)
        {
            _inputFS = _azure;

            if (String.IsNullOrEmpty(inputConnectionString))
                throw new ArgumentNullException(nameof(inputConnectionString));

            if (container == null)
                throw new ArgumentNullException(nameof(container));

            _inputFS.AddStorageAccount("azure", inputConnectionString);
            _inputFS.SetWorkingFolder($"wasb://azure/{container}");
        }

        public Stream OpenReader(string filename) => _inputFS.GetFile(filename).GetStreamReader();
        
        public Stream OpenWriter(string filename) => _inputFS.GetFile(filename).GetStreamWriter();

        public void Delete(string filename) => _inputFS.GetFile(filename).Delete();

        public void DeleteFolder(string folder) => GetFolder(folder).Delete();

        public IAzureBlobFolder GetFolder(string name)
        {
            return _inputFS.GetFolder(name);
        }

        public string[] ListFileExtension(string extension)
        {
            var inputFolder = GetFolder(@"wasb://input");

            var files = inputFolder
                .EnumItems()
                .Where(f => f.Name.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                .Select(f => f.Name)
                .ToArray();
            
            return files;
        }

        public string[] ListFolderContent(string path)
        {
            var inputFolder = GetFolder("wasb://" + path);

            var files = inputFolder
                .EnumItems()
                .Select(f => $"{path}/{f.Name}")
                .ToArray();

            return files;
        }
    }
}
