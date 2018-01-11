using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PdfTextReader.Base
{
    interface ILogStructure<T>
    {
        void StartLog(TextWriter input);
        void Log(TextWriter input, T data);
        void EndLog(TextWriter input);
    }
}
