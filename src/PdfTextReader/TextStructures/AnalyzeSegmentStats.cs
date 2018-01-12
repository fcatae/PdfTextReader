using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace PdfTextReader.TextStructures
{
    class AnalyzeSegmentStats : ILogStructure<TextSegment>
    {
        class TitleStats
        {
            public string FontName;
            public float FontSize;
            public string FontStyle;
        }

        List<TitleStats> _stats = new List<TitleStats>();

        public void StartLog(TextWriter input)
        {
        }
        
        public void Log(TextWriter input, TextSegment segment)
        {
            var items = from t in segment.Title
                        select new TitleStats
                        {
                            FontName = t.FontName,
                            FontSize = t.FontSize,
                            FontStyle = t.FontStyle
                        };

            _stats.AddRange(items);
        }

        public void EndLog(TextWriter input)
        {
            var summary = from s in _stats
                          group s by (s.FontName + " " + s.FontStyle + " " + s.FontSize.ToString("0.00")) into g
                          orderby g.Count() descending
                          select new { Font = g.Key, Count = g.Count() };                          

            foreach(var line in summary)
            {
                input.WriteLine($"[{line.Font}] = {line.Count}");
            }
        }
    }
}
