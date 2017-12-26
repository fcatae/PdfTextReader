using iText.Kernel.Pdf;
using System;

namespace PdfTextReader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var user = new UserWriter();

            string basename = "p40";
            //user.ProcessBlock($"bin/{basename}.pdf", $"bin/{basename}-output.pdf");


            user.ProcessText($"bin/{basename}.pdf", $"bin/{basename}-output.pdf");


            //user.Process("bin/p40.pdf", b =>
            //{
            //    System.Diagnostics.Debug.WriteLine(b.Text);
            //});
        }
    }
}
