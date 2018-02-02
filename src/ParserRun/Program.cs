using PdfTextReader;
using System;

namespace ParserRun
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Parser Run");

            var config = new Config(args);

            Console.WriteLine(config.Get("AZURE_STORAGE_PDF"));
            Console.WriteLine(config.Get("AZURE_STORAGE_OUTPUT"));

            // DEV: configure the secrets
            //
            //   dotnet user-secrets set AZURE_STORAGE_PDF <_connection_string_from_portal_>
            //   dotnet user-secrets set AZURE_STORAGE_OUTPUT <_connection_string_from_portal_>
            //
            string inputConnectionString = config.Get("AZURE_STORAGE_PDF");
            string outputConnectionString = config.Get("AZURE_STORAGE_OUTPUT");

            // Test azure connection
            //TestAzureBlob.V2(connectionString, storageContainer);
            //TestAzureBlob.Run(connectionString, storageContainer);
            //TestAzureBlob.Enum(connectionString, storageContainer);

            // create the AzureFS
            var azureBlobs = new AzureFS(inputConnectionString, outputConnectionString);

            //ExamplesAzure.FollowText(virtualFileSystem, "example");
            //ExamplesAzure.RunParserPDF(azureBlobs, "DO1_2010_01_04", "wasb://input/pdf/2010/2010_01_04", "wasb://output/test");
            ExamplesAzure.RunCreateArtigos(azureBlobs, "DO1_2010_01_04", "wasb://input/pdf/2010/2010_01_04", "wasb://output/test/logs", "wasb://output/test/artigos");
        }
    }
}
