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
            const string DEFAULT_PROCESS = "default";

            string folder = (args.Length >= 1) ? args[0] : DEFAULT_FOLDER;
            string outputFolder = (args.Length >= 2) ? args[1] : DEFAULT_OUTPUT;
            string processName = (args.Length >= 3) ? args[2] : DEFAULT_PROCESS;

            var processList = new Dictionary<string, IRunner>
            {
                { "default", new GeneralProcess() },
                { "xml", new ProcessXml() },
                { "2010", new Process2010() },
                { "2012", new Process2012() },
                { "2016", new Process2016() },
                { "valid2010", new Validate2010() },
                { "process", new ProcessDefault() }
            };

            // Get the runner
            var process = processList[processName];

            // Enumerate available files
            var filelist = new FileList(process.FilePattern);
            var filenames = filelist.EnumFiles(folder);

            // Run the process for all files
            var runner = new Runner();
            long timeSpent = runner.Run(process, filenames, outputFolder);

            Console.WriteLine();
            Console.WriteLine($"Total files = {filenames.Length}");
            Console.WriteLine($"Total time = {timeSpent}");
        }        
    }
}
