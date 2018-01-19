using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Validator
{
    class Runner
    {
        public long Run(IRunner runner, File[] files, string outputFolder)
        {
            var watch = Stopwatch.StartNew();

            foreach (var file in files)
            {
                Run(runner, file, outputFolder);
            }

            watch.Stop();

            return watch.ElapsedMilliseconds;
        }

        void Run(IRunner runner, File file, string outputname)
        {
            string basename = file.Filename;

            try
            {
                Console.WriteLine($"File {basename}: Start");
                runner.Run(file, outputname);
                Console.WriteLine($"File {basename}: End");
            }
            catch (Exception ex)
            {
                Console.WriteLine("CRITICAL ERROR: Unhandled Exception");
                Console.WriteLine(ex.ToString());
                Console.WriteLine();
                Console.WriteLine($"File {basename}: End with FAILURES");
            }
        }
    }
}
