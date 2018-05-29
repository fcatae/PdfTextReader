using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PdfTextReader.TextStructures
{
    class AnalyzeSegmentTextVersion : ILogStructure<TextSegment>
    {
        public void EndLog(TextWriter input)
        {
        }

        public void Log(TextWriter input, TextSegment data)
        {
            input.WriteLine(data.TitleText);
            input.WriteLine();
            input.WriteLine(data.BodyText);
            input.WriteLine();
            input.WriteLine();
        }

        public void StartLog(TextWriter input)
        {
        }
    }
}
