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
}
