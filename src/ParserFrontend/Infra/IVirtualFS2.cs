using PdfTextReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParserFrontend
{
    public interface IVirtualFS2 : IVirtualFS
    {
        string[] ListFileExtension(string extension);
        string[] ListFolderContent(string folder);
        void Delete(string filename);
    }
}
