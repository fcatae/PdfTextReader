using System;
using System.Collections.Generic;
using System.Text;
using PdfTextReader.Base;
using PdfTextReader.Parser;
using PdfTextReader.ExecutionStats;
using PdfTextReader.TextStructures;

namespace PdfTextReader
{
    class Program3
    {
        public static void ProcessTextLines(string page)
        {
            Console.WriteLine();
            Console.WriteLine("Program3 - ProcessTextLines");
            Console.WriteLine();

            var artigos = Examples.GetTextLines(page)
                            .ConvertText<CreateStructures, TextStructure>()
                            .ConvertText<CreateTextSegments, TextSegment>()
                                .DebugPrint()
                            .ToList();            
        }
    }
}
