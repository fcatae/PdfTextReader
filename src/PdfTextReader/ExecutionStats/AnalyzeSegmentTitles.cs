using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace PdfTextReader.ExecutionStats
{
    class AnalyzeSegmentTitles : ILogStructure<TextSegment>
    {
        public void StartLog(TextWriter input)
        {
        }
        
        public void Log(TextWriter input, TextSegment segment)
        {
            input.WriteLine("-----------------------------------");
            
            foreach(var title in segment.Title)
            {
                input.WriteLine($"{title.FontName}, {title.FontSize.ToString("0.00")}");
            }

            input.WriteLine("");
        }

        public void EndLog(TextWriter input)
        {
        }
    }
}
