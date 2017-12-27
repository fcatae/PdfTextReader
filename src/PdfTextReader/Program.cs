using iText.Kernel.Pdf;
using System;
using System.IO;

namespace PdfTextReader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Testing in batch!");
            //ProcessFiles();

            ProcessPage();
        }

        static void ProcessPage()
        {
            string subfolder = "PerYear";
            var dir = new DirectoryInfo($"bin/{subfolder}");

            var user = new UserWriter();

            string basename = "p44";
            user.ProcessBlock($"bin/{basename}.pdf", $"bin/{basename}-output.pdf");
            //user.ProcessText($"bin/{subfolder}/{basename}.pdf", $"bin/{subfolder}/{basename}-output.pdf");
            //user.ProcessBlock($"bin/{subfolder}/{basename}.pdf", $"bin/{subfolder}/{basename}-output.pdf");
        }

        static void ProcessFiles()
        {
            string subfolder = "Samples";
            var dir = new DirectoryInfo($"bin/{subfolder}");
            var user = new UserWriter();

            foreach (var f in dir.EnumerateFiles("*.pdf"))
            {
                string filename = f.Name;

                string basename = Path.GetFileNameWithoutExtension(filename);

                if (basename.EndsWith("-output"))
                    continue;

                user.ProcessText($"bin/{subfolder}/{basename}.pdf", $"bin/{subfolder}/{basename}-output.pdf");
            }


        }
    }
}
