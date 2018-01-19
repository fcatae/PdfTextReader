using System;
using System.Collections.Generic;
using System.Text;

namespace Validator
{
    class Process2010 : IRunner
    {
        public void Run(File file, string outputname)
        {
            string inputFolder = file.Folder;
            string basename = file.Filename;

            PdfTextReader.ProgramValidator2010.Process(basename, inputFolder, outputname);
        }
    }
}
