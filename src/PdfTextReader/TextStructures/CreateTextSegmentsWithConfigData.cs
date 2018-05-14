using PdfTextReader.TextStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;
using PdfTextReader.Configuration;

namespace PdfTextReader.Parser
{
    class CreateTextSegmentsWithConfigData : IAggregateStructure<TextStructure, TextSegment>
    {        
        private IList<string> _parserTitles;
        private int _parserTitleStart;
        private int _parserTitlePosition;
        private int _parserTitleTotal;

        public CreateTextSegmentsWithConfigData(ParserTreeConfig parserTreeConfig)
        {
            if (!parserTreeConfig.IsValid)
                PdfReaderException.AlwaysThrow("ParserTreeConfig is invalid");

            this._parserTitles = parserTreeConfig.Titles;
            this._parserTitlePosition = 0;
            this._parserTitleTotal = parserTreeConfig.Titles.Count;
        }

        bool _title = true;

        public bool Aggregate(TextStructure line)
        {
            bool isTitle = _title;
            bool isBody = (_title == false) && (!IsTitle(line.Text));

            if ((_title == true) && (!CheckAndMoveNext(line.Text)) )
            {
                _title = false;
            }
            
            return (isTitle || isBody);
        }

        public TextSegment Create(List<TextStructure> _structures)
        {
            int total = _structures.Count;

            // when page has one segment, but no title at all
            int idxTitle = _parserTitlePosition - _parserTitleStart;

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
            _parserTitleStart = _parserTitlePosition;
            _title = CheckAndMoveNext(line.Text);
        }

        bool CheckAndMoveNext(string line)
        {
            bool isTitle = IsTitle(line);

            if (isTitle)
                NextTitle();

            return isTitle;
        }

        bool CompareTitle(string a, string b)
        {
            return a.Replace("\n"," ").Replace(" ","") == b.Replace("\n"," ").Replace(" ", "");
        }

        bool IsTitle(string line)
        {
            return HasTitle() && CompareTitle(GetTitle(), line);
        }

        string GetTitle()
        {
            return _parserTitles[_parserTitlePosition];
        }

        void NextTitle()
        {
            _parserTitlePosition++;
        }

        bool HasTitle()
        {
            return _parserTitlePosition < _parserTitleTotal;
        }

    }
}
