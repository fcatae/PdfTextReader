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

        private static bool g_ShowWarnings = true;
        private static bool g_ContinueOnException = false;

        [ThreadStatic]
        private static PdfReaderExceptionContext g_threadContext;

        public static void SetContext(string filename, int pageNumber)
        {
            if (g_threadContext != null)
                throw new InvalidOperationException();

            g_threadContext = new PdfReaderExceptionContext() { Filename = filename, PageNumber = pageNumber };
        }

        public static void ClearContext()
        {
            g_threadContext = null;
        }

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
            if(g_ShowWarnings)
            {
                Console.WriteLine($"WARNING: {sourceMethod}: {message} {GetAdditionalPageInformation()}");
            }
        }

        public static void Throw(string message, [CallerFilePath]string source = null, [CallerMemberName]string sourceMethod = null)
        {
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
                    Console.WriteLine($"CRITICAL EXCEPTION: {sourceMethod} {GetAdditionalPageInformation()}");
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
            throw new PdfReaderException($"{message} {GetAdditionalPageInformation()}");
        }

        public static string GetAdditionalPageInformation()
        {
            if (g_threadContext == null)
                return "--- NOT AVAILABLE ---";

            return $"({g_threadContext.Filename}, Page {g_threadContext.PageNumber})";
        }

        class PdfReaderExceptionContext
        {
            public string Filename { get; set; }
            public int PageNumber { get; set; }
        }

    }
}
