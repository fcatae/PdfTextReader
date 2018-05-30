using System;

namespace ParserConsoleWeb
{
    class Program
    {
        static void Main(string[] args)
        {
            if( args.Length == 0 )
            {
                Console.WriteLine("Syntax: ParserConsoleWeb <inputfile> [storage_account]");
                Console.WriteLine(" - inputile: does not include the extension");
                Console.WriteLine(" - storage_account: also configurable in Environment: STORAGE_ACCOUNT");
                return;
            }

            string inputFile = args[0];
            string storageAccount = (args.Length > 1) ? args[1] : Environment.GetEnvironmentVariable("STORAGE_ACCOUNT");

            var azureFS = new AzureFS(storageAccount);

            var pdf = new PdfProcessor(azureFS);

            pdf.Process(inputFile);
        }
    }
}
