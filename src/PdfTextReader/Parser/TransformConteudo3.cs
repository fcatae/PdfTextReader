using PdfTextReader.TextStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;
using System.Text.RegularExpressions;

namespace PdfTextReader.Parser
{
    class TransformConteudo3 : IAggregateStructure<TextSegment, Conteudo>
    {
        int _conteudoId = 0;

        public void Init(TextSegment line)
        {
        }

        public bool Aggregate(TextSegment line)
        {
            return false;
        }

        public Conteudo Create(List<TextSegment> segments)
        {
            var segment = ProcessExclusiveText(segments[0]);

            int page = -1;
            if (segment.Body.Count() > 0)
                page = segment.Body[0].Lines[0].PageInfo.PageNumber;

            string titulo = null;
            string hierarchy = null;
            string body = null;
            string caput = null;
            List<Autor> autores = null;

            // Hierarquia
            var hierarquiteTitulo = segment.Title.Select(t => CleanupBreaklinesAndHyphens(t.Text)).ToArray();

            // Texto
            string texto = segment.BodyText;


            // Titulo
            int idxTitle = segment.Title.Count() - 1;
            titulo = segment.Title[idxTitle].Text;

            // Caput
            var firstLine = segment.Body
                                .Where(t => t.TextAlignment != TextAlignment.CENTER) // Skip the subtitles
                                .First();
                        
            caput = (firstLine.TextAlignment == TextAlignment.RIGHT) ? firstLine.Text : null;

            // Body
            int idxEndPos = -1;
            int idxStartPos = -1;

            for (int pos=segment.Body.Length-1; pos>=0; pos--)
            {
                var lastLine = segment.Body[pos];

                // skip CENTERed text
                while((lastLine.TextAlignment == TextAlignment.CENTER) && (pos >= 0))
                {
                    lastLine = segment.Body[pos--];
                }

                idxEndPos = pos + 1;

                // find CENTERed text
                while ((lastLine.TextAlignment != TextAlignment.CENTER) && (pos>=0))
                {
                    lastLine = segment.Body[pos--];
                }

                idxStartPos = pos + 1;

                FindSignatures(segment.Body, idxStartPos, idxEndPos);
            }

            return new Conteudo()
            {
                IntenalId = _conteudoId++,
                Page = page,
                Hierarquia = hierarchy,
                Titulo = CleanupBreaklinesAndHyphens(titulo),
                Caput = CleanupBreaklinesAndHyphens(caput),
                Corpo = ReplaceBreaklinesAndHyphensWithHtml(body),
                Autor = autores,
                Data = null,
                Anexos = null,
                HierarquiaTitulo = hierarquiteTitulo,
                Texto = texto
            };
        }

        string GenerateText(TextStructure s)
        {
            string prefix = "";

            if(s.TextAlignment == TextAlignment.JUSTIFY)
            {
                return s.Text.Replace("\t", "\n\t").TrimStart('\n');
            }

            if (s.TextAlignment == TextAlignment.LEFT || s.TextAlignment == TextAlignment.UNKNOWN)
            {
                PdfReaderException.Warning("s.TextAlignment == TextAlignment.LEFT || s.TextAlignment == TextAlignment.UNKNOWN");
            }

            if (s.TextAlignment == TextAlignment.CENTER)
                prefix = "\t\t";

            if (s.TextAlignment == TextAlignment.RIGHT)
                prefix = "\t\t\t\t";

            var lines = s.Text.Split('\n').Select(l => prefix + l);

            string text = String.Join("\n", lines);

            return text;
        }

        string CleanupHyphens(string body)
        {
            if (body == null) return null;

            if (!body.Contains("-"))
                return body;

            // Algumas vezes, a deteccao de texto indica que a continuacao
            // do hifen cai sobre o parágrafo seguinte. Nesse caso, aceitamos
            // que pode ser um erro e consideramos igual a uma quebra de linha
            string incluirQuebraParagrafo = body.Replace("-\n\n", "-\n");
            
            string texto = HifenUtil.ExtrairHifen(incluirQuebraParagrafo);

            return texto;
        }

        string CleanupBreaklines(string body)
        {
            if (body == null) return null;

            return body.Replace("\n", " ");
        }

        string CleanupBreaklinesAndHyphens(string body)
        {
            if (body == null) return null;

            return CleanupBreaklines(CleanupHyphens(body));
        }

        string ReplaceBreaklinesAndHyphensWithHtml(string body)
        {
            if (body == null) return null;

            var cleanBody = CleanupHyphens(body);
            var lines = cleanBody.Split(new string[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
            var cleanLines = lines.Select(l => CleanupBreaklines(l));

            return "<p>" + String.Join("</p><p>", cleanLines) + "</p>";
        }

        string RemoveDataFromBody(string body, string data)
        {
            return body.Replace(data, "");
        }

        string HasData(string body)
        {

            string LastLine = body.Split('\n').Last();

            string result = null;

            var match = Regex.Match(LastLine, @"(.+?[a-zA-Z]+, \d\d de [a-zA-Z]+ de \d{4})");

            if (match.Success)
                return LastLine;

            return result;
        }

        void FindSignatures(IEnumerable<TextStructure> bodyEnum, int start, int end)
        {
            var body = bodyEnum.ToArray();
            int idxStart = start;
            int idxEnd = end;

            for (int idx=end-1; idx>=start; idx--)
            {
                // search for the beginning of signatures
                if(body[idx].TextAlignment != TextAlignment.RIGHT)
                {
                    // minor exception: if the last line is JUSTIFIED
                    if (body[idx].TextAlignment == TextAlignment.JUSTIFY)
                    {
                        // only the last line
                        if (idxEnd == end && (idx == end - 1))
                        {
                            idxEnd = idx;
                            continue;
                        }
                    }

                    idxStart = idx + 1;
                    break;
                }
            }
            
            int length = idxEnd - idxStart;

            // not signature found!
            if( length == 0 )
            {
                return;
            }

            var findSignatures = body.Skip(idxStart).Take(length).ToArray();

            ProcessSignatures(findSignatures);
        }

        void ProcessSignatures(IEnumerable<TextStructure> bodyEnum)
        {

        }
        
        TextSegment ProcessExclusiveText(TextSegment segment)
        {
            foreach (TextStructure item in segment.Body)
            {
                if (item.Text.ToLower().Contains("o presidente da república") || item.Text.ToLower().Contains("a presidenta da república"))
                    item.TextAlignment = TextAlignment.JUSTIFY;
                if (item.Text.Contains("Parágrafo único"))
                {
                    if (item.Text.Substring(0, 15) == "Parágrafo único")
                        item.TextAlignment = TextAlignment.JUSTIFY;
                }
                if (item.Text.Contains("Art."))
                {
                    if (item.Text.Substring(0, 4) == "Art.")
                        item.TextAlignment = TextAlignment.JUSTIFY;
                }
            }
            return segment;
        }

        List<string> ProcessSignatureAndRole(List<TextLine> lines)
        {

            string signature = null;
            string role = null;
            string date = null;


            foreach (var item in lines)
            {
                if (item.Text.ToUpper() == item.Text)
                {
                    signature = signature + "\n" + item.Text;
                }
                else if (item.FontStyle == "Italic" || item.FontName.ToLower().Contains("italic"))
                {
                    signature = signature + "\n" + item.Text;
                }
                else
                {
                    var match = Regex.Match(item.Text, @"(\,? [0-9]* [a-zA-Z]* [a-zA-Z]* [a-zA-Z]* [0-9]*)");
                    var match2 = Regex.Match(item.Text, @"([0-9]*\/[0-9]*\/[0-9]*)");

                    if (match.Success)
                        date = item.Text;
                    else if (match2.Success)
                        date = item.Text;
                    else
                        role = role + "\n" + item.Text;
                }
            }

            return new List<string>() { signature, role, date };
        }

        List<Autor> ProcessListOfSignatures(List<TextStructure> signatures)
        {
            List<Autor> autores = new List<Autor>();
            foreach (TextStructure item in signatures)
            {
                Autor autor = new Autor();
                foreach (var line in item.Lines)
                {
                    if (line.Text.ToUpper() == line.Text)
                    {
                        if (!String.IsNullOrWhiteSpace(autor.Assinatura))
                        {
                            autor.Assinatura = line.Text;
                        }
                        else
                        {
                            autor = new Autor() { Assinatura = line.Text };
                            autores.Add(autor);
                            continue;
                        }
                    }
                    else
                    {
                        autor.Cargo = line.Text;
                    }

                    autores.Add(autor);
                }
            }
            return autores;
        }
    }
}
