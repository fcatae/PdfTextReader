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
