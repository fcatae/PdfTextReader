using PdfTextReader.TextStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.Parser
{
    class CreateTreeSegments : IAggregateStructure<TextSegment, TextSegment>
    {
        bool _shouldNotContinue = false;
        TextSegment _current = null;

        public bool Aggregate(TextSegment line)
        {
            return false;
        }

        public TextSegment Create(List<TextSegment> segmentList)
        {
            foreach(var segment in segmentList)
            {
                // merge the contexts
                if(_current != null)
                {
                    var current = _current.Title;
                    var next = segment.Title;

                    if (next == null)
                        PdfReaderException.AlwaysThrow("next == null ");

                    // The last structure may be broken
                    if (next.Length == 0)
                    {
                        // for safety
                        _shouldNotContinue = true;
                        return null;
                    }

                    var mergedSegments = MergeSegments(current, next);

                    ValidateFontStyle(mergedSegments);

                    _current = new TextSegment()
                    {
                        Title = mergedSegments,
                        Body = segment.Body,
                        TitleText = segment.TitleText,
                        BodyText = segment.BodyText,
                        OriginalTitle = segment.Title
                    };
                }
                else
                {
                    _current = segment;
                    _current.OriginalTitle = _current.Title;
                }
            }

            return _current;
        }

        public void Init(TextSegment line)
        {
            if (_shouldNotContinue)
                PdfReaderException.AlwaysThrow("_shouldNotContinue");
        }

        void ValidateFontStyle(TextStructure[] titles)
        {
            for(int i=0; i<titles.Length-1; i++)
            {
                var major = titles[i];
                var minor = titles[i + 1];

                int compare = CompareStructureHieararchy(major, minor);

                if (compare < 0)
                {
                    // Exemplo: DO1_2015_01_12, p5: "ANEXO" seguido de "Nota Tecnica" em bold.
                    PdfReaderException.Warning("invalid title order");
                    return;
                }
            }
        }

        //TextStructure[] MergeSegments(TextStructure[] current, TextStructure[] next)
        //{
        //    for(int split=1; split<next.Length; split++)
        //    {
        //        var major = next[split-1];
        //        var minor = next[split];

        //        // found a split
        //        if( CompareStructureHieararchy(major, minor) < 0 )
        //        {
        //            var orderedBlock = next.Take(split);
        //            var unorderedBlock = next.Skip(split);

        //            var merge1 = MergeSegmentsOrdered(current, orderedBlock.ToArray());

        //            return MergeSegments(merge1, unorderedBlock.ToArray());
        //        }
        //    }

        //    // lists are both ordered
        //    return MergeSegmentsOrdered(current, next);
        //}

        //TextStructure[] MergeSegmentsOrdered(TextStructure[] current, TextStructure[] next)
        TextStructure[] MergeSegments(TextStructure[] current, TextStructure[] next)
        {
            if ((current == null) || (next == null) || (next.Length == 0))
                PdfReaderException.AlwaysThrow("(current == null) || (next == null) || (next.Length == 0)");

            var headNext = next[0];
            int remainingTreeSize = -1;

            for (int i=current.Length-1; i>=0; i--)
            {
                if(CompareStructureHieararchy(current[i], headNext) > 0)
                {
                    remainingTreeSize = i + 1;
                    break;
                }
            }

            // replace the current Tree with the next Tree
            if( remainingTreeSize == -1 )
            {
                return (TextStructure[])next.Clone();
            }

            int nextTreeSize = next.Length;
            int totalTreeSize = remainingTreeSize + nextTreeSize;

            var finalStructure = current
                                    .Take(remainingTreeSize)
                                    .Concat(next)
                                    .ToArray();

            return finalStructure;
        }

        int CompareStructureHieararchy(TextStructure a, TextStructure b)
        {
            // Font size
            int compareFontSize = CompareFontSize(a.FontSize, b.FontSize);

            if (compareFontSize != 0)
                return compareFontSize;
            
            // Boldness
            int compareBoldness = CompareBoldness(a.FontStyle, b.FontStyle);

            if (compareBoldness != 0)
                return compareBoldness;

            // Compare upper case?
            return 0;
        }

        int CompareFontSize(float a, float b)
        {
            const float zero = 0.01f;

            if (Math.Abs(a - b) < zero)
                return 0;

            if (a > b)
                return 1;

            return -1;
        }

        int CompareBoldness(string a, string b)
        {
            if (a == b)
                return 0;

            if (a == "Bold")
                return 1;

            if (b == "Bold")
                return -1;

            return 0;
        }

    }
}
