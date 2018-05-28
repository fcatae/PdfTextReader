using System;
using System.IO;
using System.Diagnostics;

namespace PdfTextReader
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 2 && args[0] == "extract")
            {
                ExampleStages.ExtractHeader(args[1]);
                return;
            }

            Console.WriteLine("PDF Text Reader");
            var watch = Stopwatch.StartNew();

            Program3.ProcessStage("DO1_2017_01_02", 1);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            Console.WriteLine($"Elapsed time was: {elapsedMs}");

            Console.ReadKey();
        }
    }
}
