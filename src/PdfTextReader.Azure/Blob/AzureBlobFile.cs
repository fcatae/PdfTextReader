using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PdfTextReader.Azure.Blob
{
    public class AzureBlobFile : AzureBlobRef
    {
        public AzureBlobFile(AzureBlobRef parent, string filename) : base(parent, filename)
        {
        }

        // synchronous version
        [DebuggerHidden]
        public Stream GetStreamWriter(string filename) => GetStreamWriterAsync(filename).Result;
        [DebuggerHidden]
        public Stream GetStreamReader(string filename) => GetStreamReaderAsync(filename).Result;

        public async Task<Stream> GetStreamWriterAsync(string filename)
        {
            throw new NotImplementedException();
            //var blockBlob = _container.GetBlockBlobReference(filename);

            //await _container.CreateIfNotExistsAsync();

            //return await blockBlob.OpenWriteAsync();
        }

        public Task<Stream> GetStreamReaderAsync(string filename)
        {
            throw new NotImplementedException();
            //var blockBlob = _container.GetBlockBlobReference(filename);

            //return blockBlob.OpenReadAsync();
        }
    }
}
