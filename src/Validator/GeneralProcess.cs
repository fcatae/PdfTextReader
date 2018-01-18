using System;
using System.Collections.Generic;
using System.Text;

namespace Validator
{
    class GeneralProcess : IRunner
    {
        public void Run(File file, string outputname)
        {
            string inputFolder = file.Folder;
            string basename = file.Filename;

            Console.WriteLine($"File {basename}: Start");

            try
            {
                PdfTextReader.ProgramValidator.Process(basename, inputFolder, outputname);
            }
            catch
            {
                Console.WriteLine("CRITICAL ERROR: Unhandled Exception");
            }
            
            Console.WriteLine($"File {basename}: End");
        }
    }
}
