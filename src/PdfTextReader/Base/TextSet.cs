using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.TextStructures
{
    class TextSet
    {
        List<TextLine> _alllines = new List<TextLine>();

        public void Append(IEnumerable<TextLine> lines)
        {
            _alllines.AddRange(lines);
        }

        public IEnumerable<TextLine> AllText => _alllines;
    }
}
