using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.Parser
{
    class Anexo
    {
        public string Texto { get; set; }
        public string Titulo { get; set; }
        public string HierarquiaTitulo { get; set; }

        public Anexo (string text)
        {
            string[] lines = text.Split('\n');
            lines = CleanVoids(lines);

            HierarquiaTitulo = GetTitleTree(lines);
            Titulo = GetTitleIndex(lines) >= 0 ? lines[GetTitleIndex(lines)] : null;
            Texto = GetBody(lines);
        }

        public Anexo() { }

        string[] CleanVoids(string[] lines)
        {
            foreach (string line in lines)
            {
                if (String.IsNullOrWhiteSpace(line))
                    lines = lines.Where(l => l != line).ToArray();
            }
            return lines;
        }

        int GetTitleIndex(string[] lines)
        {
            int idxTitle = -1;
            foreach (string line in lines)
            {
                if (line.Count() > 5)
                {
                    if (line.Substring(0, 5).ToLower() == "anexo")
                        idxTitle = lines.ToList().IndexOf(line);
                }
            }
            return idxTitle;
        }

        string GetBody(string[] lines)
        {
            int title = GetTitleIndex(lines);
            if (title > 0 && title < lines.Count() - 1)
            {
                return String.Join("\n", lines.Skip(title + 1).Take(lines.Count() - title));
            }
            return null;
        }

        string GetTitleTree(string[] lines)
        {
            int title = GetTitleIndex(lines);
            if (title > 0)
            {
                return String.Join(":", lines.Take(title));
            }
            return null;
        }
    }
}
