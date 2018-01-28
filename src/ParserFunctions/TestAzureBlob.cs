using System;
using System.Collections.Generic;
using System.Text;

namespace ParserFunctions
{
    class TestAzureBlob
    {
        public static void Run(AzureBlob blob, string filename)
        {
            using (var sw = new System.IO.StreamWriter(blob.GetStreamWriter(filename)))
            {
                sw.WriteLine("Hello from WRITER");
            }

            using (var sr = new System.IO.StreamReader(blob.GetStreamReader(filename)))
            {
                string output = sr.ReadToEnd();

                Console.WriteLine("Output from READER = " + output);
            }
        }
    }
}
