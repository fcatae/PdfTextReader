using PdfTextReader.TextStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.Parser
{
    class AfterFilterTextSegments : IAggregateStructure<TextSegment, TextSegment>
    {        
        public bool Aggregate(TextSegment line)
        {
            return false;
        }

        public TextSegment Create(List<TextSegment> _structures)
        {
            return new TextSegment()
            {
                Title = _structures[0].Title,
                Body = _structures[0].Body
            };
        }

        public void Init(TextSegment line)
        {
        }
    }
}
