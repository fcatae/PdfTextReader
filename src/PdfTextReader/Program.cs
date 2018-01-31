using System;
using System.IO;
using System.Diagnostics;

namespace PdfTextReader
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("PDF Text Reader");
            var watch = Stopwatch.StartNew();

            ExamplesAzure.RunParserPDF("DO1_2010_01_04", "bin", "bin/test");

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            Console.WriteLine($"Elapsed time was: {elapsedMs}");

            Console.ReadKey();
        }
    }
}
