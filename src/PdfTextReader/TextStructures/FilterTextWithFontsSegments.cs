using PdfTextReader.TextStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;
using System.Text.RegularExpressions;

namespace PdfTextReader.Parser
{
    class FilterTextWithFontsSegments : IAggregateStructure<TextSegment, TextSegment>
    {
        TextStructure _lastIgnored = null;
        TextSegment _textSegment = null;
        TextStructure _bodyCompare = null;

        public bool Aggregate(TextSegment line)
        {
            return false;
        }

        public TextSegment Create(List<TextSegment> _structures)
        {
            var titles = _structures[0].Title;
            var body = _structures[0].Body;
            var textSegment = _textSegment;

            int total = titles.Length;

            int stop = titles
                .TakeWhile(KeepTitle(body))
                .Count();

            if (_lastIgnored != null && CompareStructureHieararchyIgnoreCase(titles[0], _lastIgnored) <= 0)
            {
                stop = 0;
            }

            int shouldSplit = titles.TakeWhile(b => CompareStructureHieararchyIgnoreCase(titles[0], b) >= 0).Count();
            if( stop == 0 && shouldSplit < total )
            {
                var lastLines = titles.Take(shouldSplit);
                var newLines = titles.Skip(shouldSplit);

                textSegment.Body = textSegment.Body.Concat(lastLines).ToArray();

                _textSegment = Create(newLines.ToArray(), body);
                return _textSegment;
            }

            if ( total == stop )
            {
                _lastIgnored = null;

                _textSegment = new TextSegment()
                {
                    Title = titles,
                    Body = body
                };
                return _textSegment;
            }

            _lastIgnored = titles.Skip(stop).FirstOrDefault();

            var titlesMovedToBody = titles.Skip(stop).ToArray();

            int validatePortariaTitle = titles.Where(t => t.Text.StartsWith("PORTARIA")).Count();
            if (validatePortariaTitle > 0)
                PdfReaderException.Warning("validtitle inside the body");

            var newTitle = titles.Take(stop).ToArray();
            var newBody = titlesMovedToBody.Concat(body).ToArray();

            _textSegment = new TextSegment()
            {
                Title = newTitle,
                Body = newBody
            };
            return _textSegment;
        }

        TextSegment Create(TextStructure[] titles, TextStructure[] body)
        {
            int total = titles.Length;

            int stop = titles
                .TakeWhile(KeepTitle(body))
                .Count();
            
            if (total == stop)
            {
                _lastIgnored = null;

                return new TextSegment()
                {
                    Title = titles,
                    Body = body
                };
            }

            _lastIgnored = titles.Skip(stop).FirstOrDefault();

            var titlesMovedToBody = titles.Skip(stop).ToArray();

            int validatePortariaTitle = titles.Where(t => t.Text.StartsWith("PORTARIA")).Count();
            if (validatePortariaTitle > 0)
                PdfReaderException.Warning("validtitle inside the body");

            var newTitle = titles.Take(stop).ToArray();
            var newBody = titlesMovedToBody.Concat(body).ToArray();

            return new TextSegment()
            {
                Title = newTitle,
                Body = newBody
            };
        }

        public void Init(TextSegment line)
        {
        }

        Func<TextStructure, bool> KeepTitle(TextStructure[] body)
        {
            // usa o sumario para definir o tamanho da fonte padrao 
            if(_bodyCompare == null)
            {
                _bodyCompare = (body.Length > 0) ? body[0] : null;
            }
            
            return title => KeepTitleCompareFonts(title, _bodyCompare);
        }
        //bool KeepTitleCompareFonts(TextStructure title, TextStructure body) => (!RemoveTitle(title)) && (CompatibleFonts(title,body));
        bool KeepTitleCompareFonts(TextStructure title, TextStructure body) => ( CompatibleFonts(title, body));
        bool RemoveTitle(TextStructure title)
        {
            return IsAnexo(title.Text) || 
                (IsNotGrade(title) && HasManyLowerCase(title.Text) ||
                IsDataEmPautaJugamento(title.Text)
                );
        }

        bool CompatibleFonts(TextStructure title, TextStructure body)
        {
            if (body == null)
                return true;

            bool titleHasLargerFonts = CompareStructureHieararchyIgnoreCase(title, body) > 0;

            return titleHasLargerFonts;
        }

        bool IsAnexo(string text)
        {
            return text.Replace(" ", "").StartsWith("ANEXO");
        }

        bool IsNotGrade(TextStructure title)
        {
            return !title.HasBackColor;
        }

        //bool HasLowerCaseExceptO(string input)
        //{
        //    string text = input.Replace("o", "");
        //    var upper = text.ToUpper();
        //    return (upper != text);
        //}

        bool HasManyLowerCase(string input)
        {
            int count = CountCaseDifference(input);

            return (count > 2);
        }

        int CountCaseDifference(string input)
        {
            string upperCase = input.ToUpper();
            int diff = 0;

            for(int i=0; i<input.Length; i++)
            {
                if (input[i] != upperCase[i])
                    diff++;
            }

            return diff;
        }
        
        readonly Regex _data = new Regex(@"DIA (\d+) DE ((JAN|FEV|MAR|ABR|MAI|JUN|JUL|AGO|SET|OUT|NOV|DEZ)\w+) DE (\d+), ÀS (\d+:\d\d) HORAS");

        bool IsDataEmPautaJugamento(string input)
        {
            var match = _data.Match(input);

            return (match.Success);
        }

        int CompareStructureHieararchyIgnoreCase(TextStructure a, TextStructure b)
        {
            // Font size
            int compareFontSize = CompareFontSize(a.FontSize, b.FontSize);

            if (compareFontSize != 0)
                return compareFontSize;

            // Boldness
            int compareBoldness = CompareBoldness(a.FontStyle, b.FontStyle);

            if (compareBoldness != 0)
                return compareBoldness;

            return 0;
            //// Compare upper case
            //int compareUppercase = CompareUppercase(a.Text, b.Text);

            //return compareUppercase;
        }

        int CompareUppercase(string a, string b)
        {
            bool isLowerA = HasManyLowerCase(a);
            bool isLowerB = HasManyLowerCase(b);

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
