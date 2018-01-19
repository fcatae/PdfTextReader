using System;
using System.Collections.Generic;
using System.Text;

namespace Validator
{
    class GeneralProcess : IRunner
    {
        public string FilePattern => "*.pdf";

        public void Run(File file, string outputname)
        {
            string inputFolder = file.Folder;
            string basename = file.Filename;

            PdfTextReader.ProgramValidator.Process(basename, inputFolder, outputname);
        }
    }
}
