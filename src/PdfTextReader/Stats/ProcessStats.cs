using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

//Temp
using PdfTextReader.Lucas_Testes.Helpers;

namespace PdfTextReader.Stats
{
    public static class ProcessStats
    {
        public static List<TextInfo> GetAllTextStyles(List<MainItem> items)
        {
            List<TextInfo> Styles = new List<TextInfo>();

            foreach (var item in items)
            {
                if (item.GetType() == typeof(TextItem))
                {
                    var tItem = item as TextItem;
                    var s = tItem.textStyle;
                    var result = Styles.Where(i => i.FontName == s.fontName && i.FontSize == Decimal.Round(Convert.ToDecimal(s.fontSize), 2)).FirstOrDefault();
                    if (result == null)
                    {
                        Styles.Add(new TextInfo(tItem));
                    }
                }
            }
            return Styles;
        }

        public static List<TextInfo> GetAllTextStyles(List<LineItem> items)
        {
            List<TextInfo> Styles = new List<TextInfo>();

            foreach (var item in items)
            {
                var result = Styles.Where(i => i.FontName == item.fontName && i.FontSize == Decimal.Round(Convert.ToDecimal(item.fontSize), 2)).FirstOrDefault();
                if (result == null)
                {
                    Styles.Add(new TextInfo(item));
                }
            }
            return Styles;
        }

        public static void PrintTextInfo(List<TextInfo> items)
        {
            foreach (TextInfo item in items)
            {
                Debug.WriteLine($"Text: {item.Text} ---- Name: {item.FontName} ---- Fontsize: {item.FontSize}");
                Console.WriteLine($"Text: {item.Text} ---- Name: {item.FontName} ---- Fontsize: {item.FontSize}");
            }
        }

        public static void PrintTextStyle(List<LineItem> lines, string text)
        {
            foreach (LineItem item in lines)
            {
                if (item.Text.ToLower() == text)
                {
                    Debug.WriteLine($"Name: {item.fontName} --- Fontsize: {item.fontSize}");
                    Console.WriteLine($"Name: {item.fontName} --- Fontsize: {item.fontSize}");
                }
            }
        }
    }
}
