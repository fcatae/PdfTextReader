using PdfTextReader.TextStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.Parser
{
    class CreateTextLineIndex : IAggregateStructure<TextLine, TextLine>
    {
        // this class does nothing
        // however, it indirectly creates an index for TextLine
        public bool Aggregate(TextLine line)
        {
            return false;
        }

        public TextLine Create(List<TextLine> lines)
        {
            return lines[0];
        }

        public void Init(TextLine line)
        {
        }
    }
}
