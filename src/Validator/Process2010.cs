using System;
using System.Collections.Generic;
using System.Text;

namespace Validator
{
    class Process2010 : IRunner
    {
        public string FilePattern => "*.pdf";

        public void Run(File file, string outputname)
        {
            string inputFolder = file.Folder;
            string basename = file.Filename;

            try
            {
                PdfTextReader.ProgramValidator2010.Process(basename, inputFolder, outputname);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
            finally
            {
                Console.WriteLine();
            }
        }
    }
}
