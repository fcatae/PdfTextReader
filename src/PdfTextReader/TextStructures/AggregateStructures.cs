using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.TextStructures
{
    class AggregateStructures : IAggregateStructure<TextStructureAgg, TextStructure>
    {
        TextStructureAgg _current;

        public bool Aggregate(TextStructureAgg line)
        {
            if(line.SameFont && line.SameSpacing && line.AlignedTabStop)
            {
                if(_current.TextStruct.TextAlignment != line.TextStruct.TextAlignment)
                {
                    if( _current.TextStruct.TextAlignment == TextAlignment.JUSTIFY || line.TextStruct.TextAlignment == TextAlignment.JUSTIFY)
                    {
                        _current.TextStruct.TextAlignment = TextAlignment.JUSTIFY;
                        line.TextStruct.TextAlignment = TextAlignment.JUSTIFY;
                    }
                }
            }

            return false;
        }

        public TextStructure Create(List<TextStructureAgg> input)
        {
            return input[0].TextStruct;
        }

        public void Init(TextStructureAgg line)
        {
            _current = line;
        }
    }
}
