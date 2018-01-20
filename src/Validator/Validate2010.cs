using System;
using System.Collections.Generic;
using System.Text;

namespace Validator
{
    class Validate2010 : IRunner
    {
        int _totalProcessed = 0;
        int _totalErrors = 0;

        //public string FilePattern => "DO1_2010_??_10.pdf"; //6
        public string FilePattern => "*.pdf";

        public void Run(File file, string outputname)
        {            
            string inputFolder = file.Folder;
            string basename = file.Filename;

            // CMD c:\pdf\output_6 c:\pdf\valid valid2010 
            int errors = PdfTextReader.ValidatorPipeline.Process(basename, inputFolder, outputname);

            _totalProcessed++;
            _totalErrors += errors;
        }
    }
}
