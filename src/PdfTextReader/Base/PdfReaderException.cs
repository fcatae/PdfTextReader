using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace PdfTextReader.Base
{
    class PdfReaderException : Exception
    {
        public PdfReaderException(string message) : base(message)
        {
        }

        private static bool g_ContinueOnException = true;

        public static void ContinueOnException()
        {
            g_ContinueOnException = true;
        }

        public static void Throw(string message, [CallerFilePath]string source = null, [CallerMemberName]string sourceMethod = null)
        {
            try
            {
                throw new PdfReaderException(message);
            }
            catch(Exception ex)
            {
                if (g_ContinueOnException)
                {
                    Console.WriteLine("=======================================");
                    Console.WriteLine($"{source}");
                    Console.WriteLine($"CRITICAL EXCEPTION: {sourceMethod}");
                    Console.WriteLine();
                    Console.WriteLine($"    {message}");
                    Console.WriteLine();
                    //Console.WriteLine("=======================================");
                    return;
                }
                throw;
            }
        }
    }
}
