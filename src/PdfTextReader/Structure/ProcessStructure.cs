using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

//Temp
using PdfTextReader.Lucas_Testes.Helpers;

namespace PdfTextReader.Structure
{
    class ProcessStructure
    {
        public List<TextLine> ProcessLine(BlockSet bset)
        {
            var items = bset.GetList();

            float minx = bset.GetX();
            float maxx = bset.GetX() + bset.GetWidth();
            float last_y = float.NaN;
            TextLine last_tl = null;

            var lines = new List<TextLine>();

            foreach(var it in items)
            {
                var bl = (PDFCore.BlockLine)it;

                var tl = new TextLine
                {
                    FontName = bl.FontName,
                    FontSize = (decimal)bl.FontSize,
                    Text = bl.Text,
                    MarginLeft = bl.GetX() - minx,
                    MarginRight = maxx - (bl.GetX() + bl.GetWidth()),
                    Breakline = 0
                };

                lines.Add(tl);

                if (tl.Text.Contains("DIRETORIA EXECUTIVA"))
                    tl = tl;

                if(last_tl != null)
                {
                    if (float.IsNaN(last_y))
                        throw new InvalidOperationException();

                    float a = bl.GetHeight();
                    float b = bl.FontSize;
                    float diff = last_y - bl.GetH();
                    last_tl.Breakline = (decimal)(last_y - bl.GetH() - bl.FontSize);
                }

                last_tl = tl;
                last_y = bl.GetH();
            }
            
            return lines.ToList();
        }
    }
}
