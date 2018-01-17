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

<<<<<<< HEAD
            Program3.ProcessWork();
=======
            //Program3.ProcessStats();

            Program4.ProcessStats("Done/2013-1");
>>>>>>> origin/master

            //Program2.MainTest();


            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            Console.WriteLine($"Elapsed time was: {elapsedMs}");

            Console.ReadKey();
        }
    }
}
