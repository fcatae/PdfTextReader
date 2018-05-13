using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace PdfTextReader.Base
{
    class PdfReaderException : Exception
    {
        public PdfReaderException(string message, string detailed) : base(detailed)
        {
            ShortMessage = message;
        }

        public PdfReaderException(string message, string detailed, IEnumerable<IBlock> debugBlocks) : base(detailed)
        {
            ShortMessage = message;
            Blocks = debugBlocks;
        }

        public IEnumerable<IBlock> Blocks = null;
        public string ShortMessage;

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
            AddWarningInformation("Warning", message);
            if (g_ShowWarnings)
            {
                Console.WriteLine($"WARNING: {sourceMethod}: {message} {GetAdditionalPageInformation()}");
            }
        }

        public static void Throw(string message, [CallerFilePath]string source = null, [CallerMemberName]string sourceMethod = null)
        {
            Throw(message, null, source, sourceMethod);
        }

        public static void Throw(string message, IEnumerable<IBlock> debugBlocks, [CallerFilePath]string source = null, [CallerMemberName]string sourceMethod = null)
        {
            try
            {
                AddWarningInformation("Throw", message);
                throw new PdfReaderException(message, $"{message} {GetAdditionalPageInformation()}", debugBlocks);
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

        [DebuggerHidden]
        public static Exception AlwaysThrow(string message, [CallerFilePath]string source = null, [CallerMemberName]string sourceMethod = null)
        {
            AddWarningInformation("AlwaysThrow", message);
            throw new PdfReaderException(message, $"{message} {GetAdditionalPageInformation()}");
        }
        [DebuggerHidden]
        public static Exception AlwaysThrow(string message, IEnumerable<IBlock> debugBlocks, [CallerFilePath]string source = null, [CallerMemberName]string sourceMethod = null)
        {
            AddWarningInformation("AlwaysThrow", message);
            throw new PdfReaderException(message, $"{message} {GetAdditionalPageInformation()}", debugBlocks);
        }

        public static string GetAdditionalPageInformation()
        {
            if (g_threadContext == null)
                return "--- NOT AVAILABLE ---";

            return $"({g_threadContext.Filename}, Page {g_threadContext.PageNumber})";
        }
        public static void AddWarningInformation(string source, string message)
        {
            if (g_threadContext == null)
                throw new InvalidOperationException(nameof(g_threadContext));

            g_threadContext.AddWarning($"{source}: {message}");
        }

        public static IEnumerable<string> GetPageWarnings()
        {
            if (g_threadContext == null)
                throw new InvalidOperationException(nameof(g_threadContext));

            return g_threadContext.Warnings;
        }

        class PdfReaderExceptionContext
        {
            public string Filename { get; set; }
            public int PageNumber { get; set; }

            List<string> _warnings = new List<string>();

            public IEnumerable<string> Warnings => _warnings;

            public void AddWarning(string message)
            {
                _warnings.Add(message);
            }
        }

    }
}
