using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ParserAzure
{
    class BlobHelper
    {
        CloudBlobClient blobClient = null;
        CloudBlobContainer container = null;
        string _conn = null;
        string _containerName = null;
        public BlobHelper(string conn, string containerName)
        {
            _conn = conn;
            _containerName = containerName;
            Reconnect(_conn, _containerName);
        }

        private void Reconnect(string conn, string containerName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(conn);
            blobClient = storageAccount.CreateCloudBlobClient();
            container = blobClient.GetContainerReference(containerName);
            container.CreateIfNotExists();
        }

        public void Write(string filename, string text)
        {
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(filename);
            blockBlob.UploadText(text);
        }

        public string Read(string filename)
        {
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(filename);

            using (var memoryStream = new MemoryStream())
            {
                blockBlob.DownloadToStream(memoryStream);
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }

        public void Remove(string file)
        {
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(file);
            blockBlob.Delete();
        }
    }
}
