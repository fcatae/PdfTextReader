using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebFrontendImages.Logic
{
    public class ImageSource
    {
        CloudBlobContainer _container;

        public ImageSource(string storageUrl)
        {
            if (String.IsNullOrEmpty(storageUrl))
                throw new ArgumentNullException(nameof(storageUrl));

            var container = new CloudBlobContainer(new Uri(storageUrl));

            _container = container;
        }

        public async Task<Stream> GetAsync(string filename)
        {
            var blob = _container.GetBlobReference(filename);
            
            return await blob.OpenReadAsync();
        }
    }
}
