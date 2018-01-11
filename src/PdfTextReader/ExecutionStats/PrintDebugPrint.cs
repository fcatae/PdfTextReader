using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace PdfTextReader.ExecutionStats
{
    class PrintDebugPrint<T> : ILogStructure<T>
    {
        string _message = typeof(T).Name;

        public void StartLog(TextWriter input)
        {
            input.WriteLine("DebugPrint: " + _message);
        }

        public void EndLog(TextWriter input)
        {            
        }

        public void Log(TextWriter input, T data)
        {
            input.WriteLine(data.ToString());
            input.WriteLine("");
        }

    }
}
