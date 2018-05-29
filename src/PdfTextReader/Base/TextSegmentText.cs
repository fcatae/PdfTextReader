using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.Base
{
    class TextSegmentText
    {
        public TextSegment OriginalTitle { get; set; }
        public TextStructure[] Title { get; set; }
        public TextStructure[] Body { get; set; }

        public string Text { get; set; }
    }
}
