using iText.Kernel.Pdf;
using System;
using System.IO;

namespace PdfTextReader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("PDF Text Reader");
            var watch = System.Diagnostics.Stopwatch.StartNew();

            //Program3.ProcessStats();

            Program4.ProcessStats("Done/2013-1");

            //Program2.MainTest();


            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            Console.WriteLine($"Elapsed time was: {elapsedMs}");

            Console.ReadKey();
        }
    }
}
