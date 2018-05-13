using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Base
{
    class BlockLine : Block
    {
        public BlockLine()
        {
        }
        public BlockLine(Block b) : base(b)
        {
        }
        public bool HasLargeSpace { get; set; }
    }
}
