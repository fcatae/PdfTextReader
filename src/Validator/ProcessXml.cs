using System;
using System.Collections.Generic;
using System.Text;

namespace Validator
{
    class ProcessXml : IRunner
    {
        public string FilePattern => "*.xml";

        public void Run(File file, string outputname)
        {
            string inputFolder = file.Folder;
            string basename = file.Filename;

            Console.WriteLine($"Output xml file = {basename}");
        }

        void ValidateArticle()
        {

        }
    }
}
