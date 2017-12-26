using System;
using System.IO;

namespace PdfTextReader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            ProcessFiles();

            //var user = new UserWriter();

            //string basename = "p40";
            //user.ProcessBlock($"bin/{basename}.pdf", $"bin/{basename}-output.pdf");            
        }

        static void ProcessFiles()
        {
            var dir = new DirectoryInfo("bin");
            var user = new UserWriter();

            foreach (var f in dir.EnumerateFiles("*.pdf"))
            {
                string filename = f.Name;

                string basename = Path.GetFileNameWithoutExtension(filename);

                user.ProcessBlock($"bin/{basename}.pdf", $"bin/{basename}-output.pdf");
            }

            
        }
    }
}
