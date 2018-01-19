using System;
using System.Collections.Generic;
using System.Text;

namespace Validator
{
    class Process2010 : IRunner
    {
        int _totalProcessed = 0;

        // public string FilePattern => "DO1_2010_0?_10.pdf"; //6
        public string FilePattern => "*.pdf";

        public void Run(File file, string outputname)
        {            
            string inputFolder = file.Folder;
            string basename = file.Filename;

            try
            {
                PdfTextReader.ProgramValidator2010.Process(basename, inputFolder, outputname);
                _totalProcessed++;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine();
                throw;
            }
            finally
            {
            }
        }
    }
}
