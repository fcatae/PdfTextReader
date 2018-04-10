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

            var input = new AzureBlobFileSystem();
            input.AddStorageAccount("input", _config.Get("INPUT_STORAGE_ACCOUNT"));
            input.SetWorkingFolder("wasb://input");
            var container = input.GetFolder("pdf");

            var itens = container.EnumItems();

            var itens_added = ProcessItensAsync(itens, "pdf").GetAwaiter().GetResult();

            Console.WriteLine($"{itens_added} message(s) added to the queue !");

        }

        private static async Task<int> ProcessItensAsync(IEnumerable<IAzureBlob> itens, string fileExtension)
        {
            var itens_added = 0;

            var queue = new AzureQueue(_config.Get("QUEUE_STORAGE_ACCOUNT"), _config.Get("QUEUE_NAME"));

            foreach (var item in itens)
            {
                if (item is IAzureBlobFolder)
                {
                    var folder = (IAzureBlobFolder)item;
                    Console.WriteLine($"Folder: {folder.Path}");
                    itens_added += await ProcessItensAsync(folder.EnumItems(), fileExtension);
                }
                else if (item is IAzureBlobFile)
                {
                    IAzureBlobFile itemFile = item as IAzureBlobFile;
                    if (string.Equals(itemFile.Extension, fileExtension, StringComparison.CurrentCultureIgnoreCase))
                    {
                        await queue.AddMessageAsync(itemFile.Uri);
                        Console.WriteLine($"Arquivo: {itemFile.Path}");
                        itens_added++;
                    }
                }
            }

            return itens_added;
        }
    }
}
