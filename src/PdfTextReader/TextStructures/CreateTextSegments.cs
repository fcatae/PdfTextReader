using PdfTextReader.TextStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.Parser
{
    class CreateTextSegment : IAggregateStructure<TextStructure, TextSegment>
    {
        bool _title = true;

        public bool Aggregate(TextStructure line)
        {
            bool isTitle = _title;
            bool isBody = (_title == false) && (line.TextAlignment != TextAlignment.CENTER);
            
            if ((_title == true) && (line.TextAlignment != TextAlignment.CENTER))
            {
                _title = false;
            }

            return (isTitle || isBody);
        }

        public TextSegment Create(List<TextStructure> _structures)
        {
            // title = 0..idxTitle
            int idxTitle = _structures.FindIndex(l => l.TextAlignment != TextAlignment.CENTER);
            int total = _structures.Count;

            var title = _structures.GetRange(0, idxTitle);
            var body = _structures.GetRange(idxTitle, total - idxTitle);
            
            return new TextSegment()
            {
                Title = title.ToArray(),
                Body = body.ToArray()
            };
        }

        public void Init(TextStructure line)
        {
            _title = (line.TextAlignment == TextAlignment.CENTER);
        }
    }
}
