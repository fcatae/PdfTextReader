using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PdfTextReader.Parser
{
    class HifenUtil
    {
        static Regex _pattern = new Regex(@"(-[mst])?(.)-\n(.)");

        public static string ExtrairHifen(string texto)
        {
            string replace = _pattern.Replace(texto, m => {
                var g = m.Groups;
                bool keep = false;

                string corpo = g[0].Value;
                bool isMesoclise = g[1].Success;
                char charMesoclise = isMesoclise ? g[1].Value[1] : '\0';
                char charAntes = g[2].Value[0];
                char charDepois = g[3].Value[0];

                if(isMesoclise)
                {
                    keep = true;
                }

                if(IsNumber(charAntes) || IsNumber(charDepois))
                {
                    keep = true;
                }

                string keepHifen = (keep) ? "-" : "";

                return corpo.Replace("-\n", keepHifen);
            });

            return replace;
        }

        static bool IsNumber(char ch)
        {
            return (ch >= '0' && ch <= '9');
        }
    }
}
