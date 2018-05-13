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

            if( total == stop )
            {
                return new TextSegment()
                {
                    Title = titles,
                    Body = body
                };
            }

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
    }
}
