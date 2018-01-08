using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.Structure
{
    class CenteredLines : IProcessText
    {
        public TextSet ProcessText(TextSet text)
        {
            var result = new TextSet();

            List<TextLine> lineset = null;

            string fontname="";
            string fontstyle ="";
            decimal fontsize= 0;
            decimal? breakline = decimal.MaxValue;
            decimal? vspacing = null;
            string lastText = "";

            foreach (var line in text.AllText)
            {
                if (lastText.Contains("ANEXO"))
                    lastText = lastText;

                lastText = line.Text;
                    
                if(( fontname != line.FontName ) ||
                    ( fontstyle != line.FontStyle ) ||
                    ( fontsize != line.FontSize ) ||
                    ((vspacing != null) && (vspacing > line.FontSize/2)) ||
                    ((line.Breakline != null) && (line.Breakline > line.FontSize/2)) ||
                    (( vspacing != null ) && ( vspacing != line.VSpacing )) )
                {
                    fontname = line.FontName;
                    fontstyle = line.FontStyle;
                    fontsize = line.FontSize;
                    breakline = line.Breakline;
                    vspacing = line.Breakline;

                    if ( lineset != null )
                    {
                        //process lineset
                        if (lineset.All(t => IsZero(t.MarginLeft - t.MarginRight))
                            && lineset.Any(t => !IsZero(t.MarginLeft))
                            )
                        {
                            //result.Append(new TextLine[] { line });
                            result.Append(lineset);
                        }
                    }

                    lineset = new List<TextLine>();
                }


                lineset.Add(line);
            }

            if (lineset != null)
            {
                //process lineset
                if (lineset.All(t => IsZero(t.MarginLeft - t.MarginRight))
                    && lineset.Any(t => !IsZero(t.MarginLeft))
                    )
                {
                    //result.Append(new TextLine[] { line });
                    result.Append(lineset);
                }
            }

            return result;
        }

        bool IsZero(decimal value)
        {
            decimal error = 0.1M;

            return ((value > -error) && (value < error));                
        }

        bool IsAlmostZero(decimal value)
        {
            decimal error = 4M;

            return ((value > -error) && (value < error));
        }

        bool IsUpperCase(string text)
        {
            var upper = text.ToUpper();
            return (upper == text);
        }
    }
}
