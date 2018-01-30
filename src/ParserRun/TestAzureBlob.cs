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
            var containers = account.EnumItems().ToList();
            var container = account.GetFolder("pdf");

            var items = container.EnumItems().ToList();
            var folder = container.GetFolder("2010");
            var subfolder = folder.GetFolder("2010_01_04");

            var files = subfolder.EnumItems().ToList();
            var file = subfolder.GetFile("DO1_2010_01_04.pdf");

            var folder2 = container.GetFolder("2010/2010_01_04");
            var file3 = container.GetFile("2010/2010_01_04/DO1_2010_01_04.pdf");

            var path = file3.Path;
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
