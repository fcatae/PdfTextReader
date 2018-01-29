using System;

namespace ParserRun
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Parser Run");

            var config = new Config(args);

            Console.WriteLine(config.Get("AZURE_STORAGE"));
            Console.WriteLine(config.Get("AZURE_STORAGE_CONTAINER"));

            // DEV: configure the secrets
            //
            //   dotnet user-secrets set AZURE_STORAGE abc
            //   dotnet user-secrets set AZURE_STORAGE_CONTAINER abc
            //
            string connectionString = config.Get("AZURE_STORAGE");
            string storageContainer = config.Get("AZURE_STORAGE_CONTAINER");

            // Test azure connection
            TestAzureBlob.Run(connectionString, storageContainer);
            TestAzureBlob.Enum(connectionString, storageContainer);

            // create the AzureFS
            var virtualFileSystem = new AzureFS(connectionString, storageContainer);

            PdfTextReader.ExamplesAzure.FollowText(virtualFileSystem, "example");
        }
    }
}
