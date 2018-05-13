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
            return (line.Title.Length == 0);
        }

        public TextSegment Create(List<TextSegment> _structures)
        {
            if( _structures.Count == 1 )
            {
                return new TextSegment()
                {
                    Title = _structures[0].Title,
                    Body = _structures[0].Body
                };
            }

            var title = _structures[0].Title;
            var body = _structures.SelectMany(s => s.Body).ToArray();

            int additionalTitles = _structures.Skip(1).Where(s => s.Title.Length > 0).Count();
            if (additionalTitles > 0)
                PdfReaderException.Throw("s.Title.Length > 0");

            return new TextSegment()
            {
                Title = title,
                Body = body
            };
        }

        public void Init(TextSegment line)
        {
        }
    }
}
