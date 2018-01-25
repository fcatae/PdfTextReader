using System;
using System.Collections.Generic;
using System.Text;

namespace Validator
{
    class Process2010 : IRunner
    {
        int _totalProcessed = 0;

        //public string FilePattern => "DO1_2010_0?_10.pdf"; //6
        public string FilePattern => "*.pdf";

        public void Run(File file, string outputname)
        {            
            string inputFolder = file.Folder;
            string basename = file.Filename;

            if (basename.Contains("DO1_2010_01_"))
                return;

            // CMD C:\PDF\2010\ c:\pdf\output 2010
            PdfTextReader.ProgramValidator2010.Process(basename, inputFolder, outputname);
            _totalProcessed++;
        }
    }
}
