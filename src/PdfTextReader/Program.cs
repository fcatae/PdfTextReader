using System;
using System.IO;

namespace PdfTextReader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            TabifyFile("dz129");
        }
        static void TabifyFile(string basename)
        {
            var user = new UserWriter();

            user.ProcessBlockExtra($"bin/{basename}.pdf", $"bin/{basename}-table-output.pdf");
        }

        static void ProcessFile(string basename)
        {
            var user = new UserWriter();
            
            user.ProcessBlock($"bin/{basename}.pdf", $"bin/{basename}-output.pdf");
        }

        static void ProcessFiles()
        {
            var dir = new DirectoryInfo("bin");
            var user = new UserWriter();

            foreach (var f in dir.EnumerateFiles("*.pdf"))
            {
                string filename = f.Name;
                
                string basename = Path.GetFileNameWithoutExtension(filename);

                if (basename.EndsWith("-output"))
                    continue;

                user.ProcessBlock($"bin/{basename}.pdf", $"bin/{basename}-output.pdf");
            }

            
        }
    }
}
