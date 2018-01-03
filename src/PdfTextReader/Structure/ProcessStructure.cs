using System;
using System.Collections.Generic;
using System.Text;

//Temp
using PdfTextReader.Lucas_Testes.Helpers;

namespace PdfTextReader.Structure
{
    public static class ProcessStructure
    {

        public static void ProcessLine(List<MainItem> items)
        {
            foreach (MainItem item in items)
            {
                TextLine line = new TextLine();
                if (items[0].GetType() == typeof(TextItem))
                {
                    var i = items[0] as TextItem;

                    line.FontName =  i.fontName;
                    line.FontSize = Decimal.Round(Convert.ToDecimal(i.fontSize), 2);
                }
            }
        }
    }
}
