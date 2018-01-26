using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PdfTextReader.Interfaces
{
    public interface IVirtualFS
    {
        Stream OpenReader(string filename);
        Stream OpenWriter(string filename);
    }
}
