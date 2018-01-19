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

        private static bool g_IgnoreEverything = true;
        private static bool g_ShowWarnings = true;
        private static bool g_ContinueOnException = false;

        public static void ContinueOnException()
        {
            g_ContinueOnException = true;
        }

        public static void DisableWarnings()
        {
            g_ShowWarnings = false;
        }

        public static void Warning(string message, [CallerMemberName]string sourceMethod = null)
        {
            if (g_IgnoreEverything)
                return;

            if(g_ShowWarnings)
            {
                Console.WriteLine($"WARNING: {sourceMethod}: {message}");
            }
        }

        public static void Throw(string message, [CallerFilePath]string source = null, [CallerMemberName]string sourceMethod = null)
        {
            if (g_IgnoreEverything)
                return;

            try
            {
                throw new PdfReaderException(message);
            }
            catch
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

        public static Exception AlwaysThrow(string message, [CallerFilePath]string source = null, [CallerMemberName]string sourceMethod = null)
        {
            if (g_IgnoreEverything)
                return;

            throw new PdfReaderException(message);
        }
    }
}
