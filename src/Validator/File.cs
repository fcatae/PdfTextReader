using System;
using System.Collections.Generic;
using System.Text;

namespace Validator
{
    class File
    {
        public File(string folder, string filename)
        {
            Folder = folder;
            Filename = filename;
        }

        public readonly string Folder;
        public readonly string Filename;
    }
}
