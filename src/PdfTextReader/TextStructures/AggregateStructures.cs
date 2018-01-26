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
            if(line.SameFont && line.SameSpacing && (line.AlignedTabStop || line.HasContinuation))
            {
                if(_current.TextStruct.TextAlignment != line.TextStruct.TextAlignment)
                {
                    if( _current.TextStruct.TextAlignment == TextAlignment.JUSTIFY || line.TextStruct.TextAlignment == TextAlignment.JUSTIFY)
                    {
                        _current.TextStruct.TextAlignment = TextAlignment.JUSTIFY;
                        line.TextStruct.TextAlignment = TextAlignment.JUSTIFY;
                    }

                    if (_current.TextStruct.TextAlignment == TextAlignment.RIGHT && line.HasContinuation )
                    {
                        bool insideDistance = _current.VerticalSpacing < 1f;

                        if(insideDistance)
                        {
                            bool confirmContinuation = ((_current.TextStruct.MarginRight < 1f) && (line.TextStruct.MarginLeft < 1f));

                            if (!confirmContinuation)
                                PdfReaderException.Warning("failure validating line continuation");

                            PdfReaderException.Warning("convert TextAlignment to JUSTIFIED");

                            _current.TextStruct.TextAlignment = TextAlignment.JUSTIFY;
                            line.TextStruct.TextAlignment = TextAlignment.JUSTIFY;
                        }
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
