using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Execution
{
    class PipelineInputCache<TD> where TD: class
    {
        Dictionary<Type, Document> _dictDocuments = new Dictionary<Type, Document>();
        int _numberOfPages = -1;

        public void SetSize(int size)
        {
            if (size <= 0)
                PdfReaderException.AlwaysThrow("Invalid size");

            _numberOfPages = size;
        }

        Document GetCache<T>()
        {
            _dictDocuments.TryGetValue(typeof(T), out Document cache);

            if(cache == null)
            {
                cache = new Document(_numberOfPages);
                _dictDocuments[typeof(T)] = cache;
            }

            return cache;
        }

        public TD FromCache<T>(int pageNumber)
        {
            var cache = GetCache<T>();
            return cache[pageNumber];
        }

        public void StoreCache<T>(int pageNumber, TD result)
        {
            var cache = GetCache<T>();
            cache[pageNumber] = result;
        }

        class Document : List<TD>
        {
            public Document(int size) : base(new TD[size]) {}
        }
    }
}
