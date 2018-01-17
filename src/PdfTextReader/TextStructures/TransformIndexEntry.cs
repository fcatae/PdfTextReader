using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.TextStructures
{
    class TransformIndexEntry<TI, TO>
    {
        public int Id { get; set; }
        public TO Key { get; set; }
        public TI Start { get; set; }
        public TI End { get; set; }
        public List<TI> Items { get; set; }
    }
    class TransformIndexEntry2<T>
    {
        public int Id { get; set; }
        public T Key { get; set; }
        public int StartId { get; set; }
        public int EndId { get; set; }
    }
}
