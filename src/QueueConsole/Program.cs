using Microsoft.WindowsAzure.Storage;
using PdfTextReader.Azure;
using QueueConsole.Queue;
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

            string INPUT_STORAGE_ACCOUNT = _config.Get("INPUT_STORAGE_ACCOUNT");
            string QUEUE_STORAGE_ACCOUNT = _config.Get("QUEUE_STORAGE_ACCOUNT");
            string QUEUE_NAME = _config.Get("QUEUE_NAME");

            (new MainPdfToImage()).Run(INPUT_STORAGE_ACCOUNT, QUEUE_STORAGE_ACCOUNT, QUEUE_NAME);
        }
    }
}
