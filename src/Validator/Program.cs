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

            const string DEFAULT_FOLDER = "bin/input";
            const string DEFAULT_OUTPUT = "bin/output";

            string folder = (args.Length >= 1) ? args[0] : DEFAULT_FOLDER;
            string outputFolder = (args.Length >= 2) ? args[1] : DEFAULT_OUTPUT;

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
