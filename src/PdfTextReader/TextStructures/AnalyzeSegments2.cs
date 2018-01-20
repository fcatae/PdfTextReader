using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PdfTextReader.TextStructures
{
    class AnalyzeSegments2 : ILogStructure<TextSegment>
    {
        public void EndLog(TextWriter input)
        {
        }

        public void Log(TextWriter input, TextSegment data)
        {
            input.WriteLine("Text,FontName,FontSize,FontStyle,MarginLeft,MarginRight,TextAlignment,AfterSpace");
            if (data.Title.Length > 0)
            {
                input.WriteLine(data.Title.LastOrDefault().Text);
            }
            foreach (var item in data.Body)
            {
                input.WriteLine($"{item.Text.Replace(",",";")},{item.FontName},{item.FontSize},{item.FontStyle},{item.MarginLeft},{item.MarginRight},{item.TextAlignment},{item.AfterSpace}");
            }

            input.WriteLine("");
            input.WriteLine("--,--,--,--,--,--,--,--,");
            input.WriteLine("--,--,--,--,--,--,--,--,");
            input.WriteLine("");
        }

        public void StartLog(TextWriter input)
        {
        }
    }
}
