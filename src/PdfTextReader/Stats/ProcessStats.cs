using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PdfTextReader.Stats
{
    static class ProcessStats
    {
        static TextInfo GridStyle;

        public static List<TextInfo> GetAllTextInfo(IEnumerable<Structure.TextLine> lines)
        {
            List<Stats.TextInfo> Styles = new List<Stats.TextInfo>();

            foreach (Structure.TextLine line in lines)
            {
                var result = Styles.Where(i => i.FontName == line.FontName && i.FontStyle == line.FontStyle && i.FontSize == Decimal.Round(Convert.ToDecimal(line.FontSize), 2)).FirstOrDefault();
                if (result == null)
                {
                    Styles.Add(new Stats.TextInfo(line));
                }
            }
            return Styles;
        }

        public static void SetGridStyle(List<TextInfo> infos)
        {
            var result = infos.Where(i => i.FontName.ToLower().Contains("times"));
            GridStyle = infos.Except(result).ToList().FirstOrDefault();
        }

        public static TextInfo GetGridStyle()
        {
            return GridStyle;
        }

        public static void PrintTextInfo(List<TextInfo> items)
        {
            foreach (TextInfo item in items)
            {
                Debug.WriteLine($"Text: {item.Text} ---- Name: {item.FontName} ---- Fontsize: {item.FontSize}");
                Console.WriteLine($"Text: {item.Text} ---- Name: {item.FontName} ---- Fontsize: {item.FontSize}");
            }
        }

        public static void PrintAnalytics(string pdfname, IEnumerable<Structure.TextLine> lines, List<Structure.TextStructure> structures, List<Parser.TextContent> contents)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter($"bin/{pdfname}-Analytics.txt"))
            {
                file.WriteLine("<<<<<>>>>>>>>>>>>>");
                file.WriteLine("START ANALYTICS");
                file.WriteLine("<<<<<>>>>>>>>>>>>>");
                file.WriteLine("");
                file.WriteLine("");


                file.WriteLine("<<<<<>>>>>>>>>>>>>");
                file.WriteLine("PROCESSING LINES");
                file.WriteLine("<<<<<>>>>>>>>>>>>>");
                file.WriteLine("");
                file.WriteLine("");
                foreach (Structure.TextLine line in lines)
                {
                    file.WriteLine($"Breakline: {line.Breakline}");
                    file.WriteLine($"FontName: {line.FontName}");
                    file.WriteLine($"FontSize: {line.FontSize}");
                    file.WriteLine($"FontStyle: {line.FontStyle}");
                    file.WriteLine($"MarginLeft: {line.MarginLeft}");
                    file.WriteLine($"MarginRight: {line.MarginRight}");
                    file.WriteLine($"Text: {line.Text}");
                    file.WriteLine("");
                    file.WriteLine("-----");
                }



                file.WriteLine("");
                file.WriteLine("");
                file.WriteLine("<<<<<>>>>>>>>>>>>>");
                file.WriteLine("PROCESSING PARAGRAPHS");
                file.WriteLine("<<<<<>>>>>>>>>>>>>");
                file.WriteLine("");
                file.WriteLine("");
                foreach (Structure.TextStructure structure in structures)
                {
                    file.WriteLine($"TextAlignment: {structure.TextAlignment}");
                    file.WriteLine($"FontName: {structure.FontName}");
                    file.WriteLine($"FontSize: {structure.FontSize}");
                    file.WriteLine($"FontStyle: {structure.FontStyle}");
                    file.WriteLine($"Text: {structure.Text}");
                    file.WriteLine("");
                    file.WriteLine("-----");
                }



                file.WriteLine("");
                file.WriteLine("");
                file.WriteLine("<<<<<>>>>>>>>>>>>>");
                file.WriteLine("PROCESSING CONTENTS");
                file.WriteLine("<<<<<>>>>>>>>>>>>>");
                file.WriteLine("");
                file.WriteLine("");
                foreach (Parser.TextContent content in contents)
                {
                    file.WriteLine($"TextAlignment: {content.TextAlignment}");
                    file.WriteLine($"FontName: {content.FontName}");
                    file.WriteLine($"FontSize: {content.FontSize}");
                    file.WriteLine($"FontStyle: {content.FontStyle}");
                    file.WriteLine($"Text: {content.Text}");
                    file.WriteLine($"Content-Type: {content.ContentType}");
                    file.WriteLine("");
                    file.WriteLine("-----");
                }

                file.WriteLine("<<<<<>>>>>>>>>>>>>");
                file.WriteLine("END ANALYTICS");
                file.WriteLine("<<<<<>>>>>>>>>>>>>");

            }
        }
    }
}
