using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

            var runner2 = runner as IRunner2;
            if( runner2 != null )
            {
                runner2.Close(outputFolder);
            }

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
                LogException(Console.Out, basename, ex);

                using (var outfile = new StreamWriter($"{outputname}/{basename}-crash-exception.log"))
                {
                    LogException(outfile, basename, ex);
                }
            }
        }

        void LogException(TextWriter writer, string basename, Exception ex)
        {
            writer.WriteLine("CRITICAL ERROR: Unhandled Exception");
            writer.WriteLine(ex.ToString());
            writer.WriteLine();
            writer.WriteLine($"File {basename}: End with FAILURES");
        }
    }
}
