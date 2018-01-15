using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace PdfTextReader.TextStructures
{
    class AnalyzeStructuresCentral : ILogStructure<TextStructure>
    {
        public void StartLog(TextWriter input)
        {
        }
        
        public void Log(TextWriter input, TextStructure structure)
        {
            const float MARGIN_LIMIT = 1f;

            if ((structure.MarginLeft > MARGIN_LIMIT) && (structure.MarginRight > MARGIN_LIMIT))
            {
                if (structure.TextAlignment != TextAlignment.CENTER)
                {
                    float marginDiff = structure.MarginLeft - structure.MarginRight;
                    int marginDiffPercent = (int)(100 * marginDiff / structure.MarginRight);
                    bool allowedMargin = (Math.Abs(marginDiff) < structure.FontSize);
                    bool isUpperCase = IsUpperCase(structure.Text);
                    bool isFontBold = structure.FontStyle == "Bold";

                    bool shouldBeCentralized = (isUpperCase || isFontBold || allowedMargin);

                    if(shouldBeCentralized)
                    {
                        string upperCaseMessage = (isUpperCase) ? "UPPER CASE: " : "";

                        input.WriteLine("-----------------------------------");
                        input.WriteLine(structure.Text);
                        input.WriteLine();

                        input.WriteLine($"Aligment: {structure.TextAlignment}");
                        input.WriteLine($" {upperCaseMessage}Margin difference = {marginDiff}/{structure.MarginRight} ({marginDiffPercent}%)");
                        input.WriteLine($" ({structure.MarginLeft}, {structure.MarginRight})");
                        input.WriteLine($" ({structure.FontName}, {structure.FontSize.ToString("0.00")}, {structure.FontStyle})");
                    }
                }
            }
            
            input.WriteLine();
        }

        public void EndLog(TextWriter input)
        {
        }

        bool IsUpperCase(string line)
        {
            string upper = line.ToUpperInvariant();
            char char_exception = '\0';
            int errors = 0;

            for(int i=0; i<line.Length; i++)
            {
                char ch = line[i];
                char up = upper[i];

                if( ch != up )
                {
                    char_exception = ch;
                    errors++;
                }
            }

            if (errors == 0)
                return true;

            if (errors == 1 && char_exception == 'o')
                return true;

            return false;
        }
    }
}
