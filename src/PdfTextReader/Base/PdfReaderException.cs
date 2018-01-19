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

        private static bool g_ContinueOnException = false;

        public static void ContinueOnException()
        {
            g_ContinueOnException = true;
        }

        public static void Throw(string message, string details, [CallerMemberName]string source = null)
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
                    Console.WriteLine($"CRITICAL EXCEPTION: {source} ({message})");
                    Console.WriteLine();
                    Console.WriteLine($"    {details}");
                    Console.WriteLine();
                    Console.WriteLine("=======================================");
                    Console.WriteLine(ex.StackTrace);
                    return;
                }
            }
        }
    }
}
