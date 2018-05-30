using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO.Compression;
using System.IO;

namespace ParserFrontend.Infra
{
    public class ZipCompression : IDisposable
    {
        MemoryStream _memoryStream;
        ZipArchive _zipArchive;

        public ZipCompression()
        {
            _memoryStream = new MemoryStream();
            _zipArchive = new ZipArchive(_memoryStream, ZipArchiveMode.Create);
        }

        public void Add(string filename, Stream stream)
        {
            if (_zipArchive == null)
                throw new InvalidOperationException("Object disposed");

            var entry = _zipArchive.CreateEntry(filename);

            using (var destStream = entry.Open())
            {
                stream.CopyTo(destStream);
            }   
        }

        public void CopyTo(Stream stream)
        {
            _memoryStream.CopyTo(stream);
        }

        public void Dispose()
        {
            if(_zipArchive != null)
            {
                _zipArchive.Dispose();
                _zipArchive = null;
            }

            _memoryStream = null;            
        }
    }
}
