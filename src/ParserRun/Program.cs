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

            var blob = new AzureBlob(connectionString, storageContainer);
            
            using (var sw = new System.IO.StreamWriter(blob.GetStreamWriter("teste1.txt")))
            {
                sw.WriteLine("Hello from WRITER");
            }

            using (var sr = new System.IO.StreamReader(blob.GetStreamReader("teste1.txt")))
            {
                string output = sr.ReadToEnd();

                Console.WriteLine("Output from READER = " + output);
            }
        }
    }
}
