using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PdfTextReader
{
    public interface IVirtualFS
    {
        Stream OpenReader(string filename);
        Stream OpenWriter(string filename);
    }
}
