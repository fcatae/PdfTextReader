using System;
using System.Collections.Generic;
using System.Text;

namespace Validator
{
    class Process2010 : IRunner
    {
        int _totalProcessed = 0;

        public string FilePattern => "*.pdf";

        public void Run(File file, string outputname)
        {
            if (_totalProcessed > 3)
            {
                Console.WriteLine("Too much load - ignoring file");
                return;
            }

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
                throw;
            }
            finally
            {
                Console.WriteLine();
            }
        }
    }
}
