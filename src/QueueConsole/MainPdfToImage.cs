using PdfTextReader.Azure;
using QueueConsole.Queue;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QueueConsole
{
    class MainPdfToImage
    {
        string _inputStorageAccount;
        string _queueStorageAccount;
        string _queueName;

        public void Run(string inputStorageAccount, string queueStorageAccount, string queueName)
        {
            _inputStorageAccount = inputStorageAccount;
            _queueStorageAccount = queueStorageAccount;
            _queueName = queueName;

            var input = new AzureBlobFileSystem();
            input.AddStorageAccount("input", inputStorageAccount);
            input.SetWorkingFolder("wasb://input");
            var container = input.GetFolder("pdf");

            var itens = container.EnumItems();

            var itens_added = ProcessItensAsync(itens, "pdf").GetAwaiter().GetResult();

            Console.WriteLine($"{itens_added} message(s) added to the queue !");

        }

        private async Task<int> ProcessItensAsync(IEnumerable<IAzureBlob> itens, string fileExtension)
        {
            var itens_added = 0;

            var queue = new AzureQueue(_queueStorageAccount, _queueName);

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
