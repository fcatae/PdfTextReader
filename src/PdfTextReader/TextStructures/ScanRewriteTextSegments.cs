using PdfTextReader.TextStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;
using System.Text.RegularExpressions;
using PdfTextReader.Configuration;

namespace PdfTextReader.Parser
{
    class ScanRewriteTextSegments : IAggregateStructure<TextSegment, TextSegment>
    {
        private readonly ParserTreeConfig _parserTree;
        TextSegment _textSegment = null;

        public ScanRewriteTextSegments(ParserTreeConfig parserTree)
        {
            _parserTree = parserTree;
        }

        public bool Aggregate(TextSegment line)
        {
            return false;
        }

        public TextSegment Create(List<TextSegment> _structures)
        {
            var titles = _structures[0].Title;
            var body = _structures[0].Body;
            var textSegment = _textSegment;

            foreach(var title in titles)
            {

            }

            int total = titles.Length;

            _textSegment = new TextSegment()
            {
                Title = titles,
                Body = body
            };
            return _textSegment;
        }

        public void Init(TextSegment line)
        {
        }
    }
}
