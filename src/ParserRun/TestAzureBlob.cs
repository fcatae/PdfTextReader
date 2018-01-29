using PdfTextReader.Azure.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParserRun
{
    class TestAzureBlob
    {
        public static void V2(string connectionString, string storageContainer)
        {
            string accountAlias = "storage011";

            var account = new AzureBlobAccount(connectionString, accountAlias);

            var path = account.Path;
            var name = account.Name;
            //container.Name;

        }

        public static void Run(string connectionString, string storageContainer)
        {
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

        public static void Enum(string connectionString, string storageContainer)
        {
            var blob = new AzureBlob(connectionString, storageContainer);
            var files = blob.EnumerateFiles("2010/", "2010_01").ToList();
        }
    }
}
