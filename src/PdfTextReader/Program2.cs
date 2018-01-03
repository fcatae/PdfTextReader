using iText.Kernel.Pdf;
using System;
using System.IO;

namespace PdfTextReader
{
    class Program2
    {
        static void Main(string[] args)
        {
            var pipeline = new Pipeline();
            pipeline.Run("p44");
        }        
    }
}
