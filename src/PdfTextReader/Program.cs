using System;

namespace PdfTextReader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var user = new UserWriter();
            user.Process("bin/p40.pdf");
        }
    }
}
