using PdfTextReader;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParserFunctions
{
    class TestAzureBlob
    {
        public static void Run(AzureFS azure, string filename)
        {
            using (var sw = new System.IO.StreamWriter(azure.OpenWriter(filename)))
            {
                sw.WriteLine("Hello from WRITER");
            }

            using (var sr = new System.IO.StreamReader(azure.OpenReader(filename)))
            {
                string text = sr.ReadToEnd();

                Console.WriteLine("Output from READER = " + text);
            }
        }
    }
}
