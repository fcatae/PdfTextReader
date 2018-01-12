using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace PdfTextReader.TextStructures
{
    class AnalyzeLines : ILogStructure<TextLine>
    {
        public void StartLog(TextWriter input)
        {
        }
        
        public void Log(TextWriter input, TextLine line)
        {
            input.WriteLine("-----------------------------------");

            float? afterSpace = line.AfterSpace;

            input.WriteLine($"Margins: ({line.MarginLeft}, {line.MarginRight})");

            input.Write(line.Text);
            input.WriteLine($" ({line.FontName}, {line.FontSize.ToString("0.00")}, {line.FontStyle})");
            input.WriteLine($" ({afterSpace})");
            input.WriteLine();

            input.WriteLine("");
        }

        public void EndLog(TextWriter input)
        {
        }
    }
}
