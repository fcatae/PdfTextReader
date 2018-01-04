using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Parser
{
    class ProcessParser
    {
        decimal Tolerance = 3;
        public List<TextContent> ProcessStructures(List<Structure.TextStructure> structures)
        {
            List<TextContent> contents = new List<TextContent>();
            foreach (Structure.TextStructure structure in structures)
            {
                if (structure.CountLines() == 1 && structure.TextAlignment == Structure.TextAlignment.RIGHT && structure.Lines[0].MarginRight > Tolerance && structure.Text.ToUpper() == structure.Text)
                {
                    contents.Add(new TextContent(structure, ContentType.Signature));
                }
                else if (structure.CountLines() == 1 && structure.TextAlignment == Structure.TextAlignment.RIGHT && structure.Lines[0].MarginRight > Tolerance)
                {
                    contents.Add(new TextContent(structure, ContentType.Role));
                }
                else if (structure.CountLines() > 1 && structure.TextAlignment == Structure.TextAlignment.JUSTIFY)
                {
                    contents.Add(new TextContent(structure, ContentType.Body));
                }
                else if (structure.CountLines() == 1 && structure.TextAlignment == Structure.TextAlignment.RIGHT && structure.Lines[0].MarginRight < Tolerance)
                {
                    contents.Add(new TextContent(structure, ContentType.Caput));
                }
                else if (structure.TextAlignment == Structure.TextAlignment.CENTER && structure.FontStyle == "Bold")
                {
                    if (!structure.FontName.ToLower().Contains("times")) // preciso pegar do Stats
                    {
                        contents.Add(new TextContent(structure, ContentType.Grid));
                    }
                    else if (structure.FontSize > 9) // Preciso pegar do Statsrser
                    {
                        contents.Add(new TextContent(structure, ContentType.Sector));
                    }
                    else
                    {
                        contents.Add(new TextContent(structure, ContentType.Title));
                    }
                }
                else if (structure.TextAlignment == Structure.TextAlignment.CENTER && structure.Text.ToUpper() != structure.Text)
                {
                    contents.Add(new TextContent(structure, ContentType.Date));
                }
                else if (structure.TextAlignment == Structure.TextAlignment.CENTER)
                {
                    contents.Add(new TextContent(structure, ContentType.Office));
                }
            }
            return contents;
        }
    }
}
