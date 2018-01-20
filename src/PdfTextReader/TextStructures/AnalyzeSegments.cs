using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PdfTextReader.TextStructures
{
    class AnalyzeSegments : ILogStructure<TextSegment>
    {
        public void EndLog(TextWriter input)
        {
        }

        public void Log(TextWriter input, TextSegment data)
        {
            input.WriteLine("-----------------------------------");

            input.WriteLine($"Title Count: {data.Title.Length}");
            input.WriteLine($"Body Count: {data.Body.Length}");
            input.WriteLine();

            input.WriteLine($"Body Alignments: ");
            foreach (var item in data.Body)
            {
                input.WriteLine($"Text: {item.Text} ==>> {item.TextAlignment}");
            }

            input.WriteLine("");
        }

        public void StartLog(TextWriter input)
        {
        }
    }
}
