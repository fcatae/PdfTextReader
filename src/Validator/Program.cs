using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Validator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Validator v1");
            Console.WriteLine("============");
            Console.WriteLine();

            string folder = "bin/input";
            string outputFolder = "bin/output";

            var process = new GeneralProcess();
            var filelist = new FileList(folder);

            var filenames = filelist.RecursiveEnumFiles();
            long timeSpent = Run(process, filenames, outputFolder);

            Console.WriteLine();
            Console.WriteLine($"Total files = {filenames.Length}");
            Console.WriteLine($"Total time = {timeSpent}");
        }
        
        static long Run(IRunner runner, File[] files, string outputFolder)
        {
            var watch = Stopwatch.StartNew();

            foreach(var file in files)
            {
                runner.Run(file, outputFolder);
            }

            watch.Stop();

            return watch.ElapsedMilliseconds;
        }
    }
}
