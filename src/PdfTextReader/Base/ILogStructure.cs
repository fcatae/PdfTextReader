using PdfTextReader.Execution;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PdfTextReader.Base
{
    interface ILogMultipleStructure<T>
    {        
        string CreateId(T data);
        void Log(string id, Stream input, T data);
    }

    interface ILogStructure<T>
    {
        void StartLog(TextWriter input);
        void Log(TextWriter input, T data);
        void EndLog(TextWriter input);
    }

    interface ILogStructure2<T> : ILogStructure<T>
    {
        void Init(ITransformIndexTree index);
    }
}
