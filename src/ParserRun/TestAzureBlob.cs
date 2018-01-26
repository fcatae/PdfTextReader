using System;
using System.Collections.Generic;
using System.Text;

namespace ParserRun
{
    class TestAzureBlob
    {
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
    }
}
