using PdfTextReader.TextStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.Parser
{
    class FilterTextSegments : IAggregateStructure<TextSegment, TextSegment>
    {
        TextStructure _lastIgnored = null;

        public bool Aggregate(TextSegment line)
        {
            return false;
        }

        public TextSegment Create(List<TextSegment> _structures)
        {
            var titles = _structures[0].Title;
            var body = _structures[0].Body;

            int total = titles.Length;

            int stop = titles
                .TakeWhile(KeepTitle)
                .Count();

            if (_lastIgnored != null && CompareStructureHieararchy(titles[0], _lastIgnored) <= 0)
            {
                stop = 0;
            }

            if ( total == stop )
            {
                _lastIgnored = null;

                return new TextSegment()
                {
                    Title = titles,
                    Body = body
                };
            }

            _lastIgnored = titles.Skip(stop).FirstOrDefault();

            var newTitle = titles.Take(stop).ToArray();
            var newBody = titles.Skip(stop).Concat(body).ToArray();

            return new TextSegment()
            {
                Title = newTitle,
                Body = newBody
            };
        }

        public void Init(TextSegment line)
        {
        }

        bool KeepTitle(TextStructure title) => !RemoveTitle(title);
        bool RemoveTitle(TextStructure title)
        {
            return IsAnexo(title.Text) || 
                (IsNotGrade(title) && HasLowerCaseExceptO(title.Text));
        }

        bool IsAnexo(string text)
        {
            return text.Replace(" ", "").StartsWith("ANEXO");
        }

        bool IsNotGrade(TextStructure title)
        {
            return !title.HasBackColor;
        }

        bool HasLowerCaseExceptO(string input)
        {
            string text = input.Replace("o", "");
            var upper = text.ToUpper();
            return (upper != text);
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

            // Compare upper case
            int compareUppercase = CompareUppercase(a.Text, b.Text);

            return compareUppercase;
        }

        int CompareUppercase(string a, string b)
        {
            bool isLowerA = HasLowerCaseExceptO(a);
            bool isLowerB = HasLowerCaseExceptO(b);

            if (isLowerA == isLowerB)
                return 0;

            return (isLowerA) ? -1 : 1;
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
