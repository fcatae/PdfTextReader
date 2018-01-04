using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PdfTextReader.Stats
{
    public static class ProcessStats
    {
        static TextInfo GridStyle;

        public static List<TextInfo> GetAllTextInfo(List<Structure.TextLine> lines)
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
            GridStyle =  infos.Except(result).ToList().FirstOrDefault();
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
    }
}
