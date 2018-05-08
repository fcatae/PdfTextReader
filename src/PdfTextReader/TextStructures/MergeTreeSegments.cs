using PdfTextReader.TextStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.Parser
{
    class MergeTreeSegments : IAggregateStructure<TextSegment, TextSegment>
    {
        bool _shouldNotContinue = false;
        TextSegment _current = null;

        public bool Aggregate(TextSegment line)
        {
            foreach(var s in line.OriginalTitle)
            {
                if (s.Text.StartsWith("ANEXO"))
                    return true;
            }

            return false;
        }

        public TextSegment Create(List<TextSegment> segmentList)
        {
            int total = segmentList.Count;

            IEnumerable<TextStructure> body = segmentList[0].Body;

            for (int count=1; count<total; count++)
            {
                body = body.Concat(segmentList[count].OriginalTitle);
                body = body.Concat(segmentList[count].Body);
            }

            var mergedSegment = new TextSegment
            {
                Title = segmentList[0].Title,
                Body = body.ToArray(),
                OriginalTitle = segmentList[0].OriginalTitle
            };

            return mergedSegment;
        }

        public void Init(TextSegment line)
        {
        }
    }
}
