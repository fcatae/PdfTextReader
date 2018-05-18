using Microsoft.WindowsAzure.Storage;
using PdfTextReader.Azure;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QueueConsole
{
    class Program
    {
        static Config _config = null;

        static void Main(string[] args)
        {
            Console.WriteLine("Console Queue");

            _config = new Config(args);

            string INPUT_STORAGE_ACCOUNT = _config.TryGet("INPUT_STORAGE_ACCOUNT");
            string QUEUE_STORAGE_ACCOUNT = _config.TryGet("QUEUE_STORAGE_ACCOUNT");
            string QUEUE_NAME = _config.TryGet("QUEUE_NAME");

            string QUEUE_SAS = _config.TryGet("QUEUE_SAS");

            if( !String.IsNullOrWhiteSpace(INPUT_STORAGE_ACCOUNT) )
            {
                (new MainPdfToImage()).Run(INPUT_STORAGE_ACCOUNT, QUEUE_STORAGE_ACCOUNT, QUEUE_NAME);
            }
            else
            {
                (new MainConsole()).Run(QUEUE_SAS);
            }
        }
    }
}
