using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace PdfTextReader.ExecutionStats
{
    class PrintDebugCount<T> : ILogStructure<T>
    {
        string _message = typeof(T).Name;
        int _count = 0;

        public void StartLog(TextWriter input)
        {
        }

        public void EndLog(TextWriter input)
        {
            input.WriteLine(_message + ": " + _count);
        }

        public void Log(TextWriter input, T data)
        {
            _count++;
        }

    }
}
