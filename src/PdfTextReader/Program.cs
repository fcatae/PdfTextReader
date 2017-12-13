using System;

namespace PdfTextReader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var user = new UserWriter();

            user.ProcessMarker("bin/p40.pdf", "bin/output.pdf");

            //user.Process("bin/p40.pdf", b => {
            //    System.Diagnostics.Debug.WriteLine(b.Text);
            //});            
        }
    }
}
