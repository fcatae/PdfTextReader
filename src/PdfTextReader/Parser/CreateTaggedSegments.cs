using PdfTextReader.TextStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;
using System.Text.RegularExpressions;

namespace PdfTextReader.Parser
{
    class CreateTaggedSegments : IAggregateStructure<TextSegment, TextTaggedSegment>
    {
        int _conteudoId = 0;

        public void Init(TextSegment line)
        {
        }

        public bool Aggregate(TextSegment line)
        {
            return false;
        }

        public TextTaggedSegment Create(List<TextSegment> segments)
        {
            int titlePosition = segments[0].Title.Length - 1;

            var taggedTitles = segments[0].Title.Select(t =>
                                    new TextTaggedStructure
                                    {
                                        TextStructure = t,
                                        Tag = TaggedSegmentEnum.Hierarquia,
                                        TextAlignment = TextAlignment.CENTER
                                    });

            var taggedBody = segments[0].Body.Select(b =>
                                    new TextTaggedStructure
                                    {
                                        TextStructure = b,
                                        Tag = TaggedSegmentEnum.None,
                                        TextAlignment = b.TextAlignment
                                    });

            var taggedSegment = new TextTaggedSegment
            {
                OriginalSegment = segments[0],
                Title = taggedTitles.Take(titlePosition).ToArray(),
                Body = taggedTitles.Skip(titlePosition).Concat(taggedBody).ToArray()
            };

            var segmentBody = taggedSegment.Body;

            // Titulo
            segmentBody[0].Tag = TaggedSegmentEnum.Titulo;

            // Caput
            var firstLine = segmentBody
                                .Where(t => t.TextAlignment != TextAlignment.CENTER) // Skip the subtitles
                                .First();

            if (firstLine.TextAlignment == TextAlignment.RIGHT)
                firstLine.Tag = TaggedSegmentEnum.Ementa;

            // Body
            int idxEndPos = -1;
            int idxStartPos = -1;

            // Find Signatures
            for (int pos=segmentBody.Length-1; pos>=0; pos--)
            {
                var lastLine = segmentBody[pos];

                // skip CENTERed text
                while((lastLine.TextAlignment == TextAlignment.CENTER) && (pos >= 0))
                {
                    lastLine = segmentBody[pos--];
                }

                idxEndPos = pos + 1;

                // find CENTERed text
                while ((lastLine.TextAlignment != TextAlignment.CENTER) && (pos>=0))
                {
                    lastLine = segmentBody[pos--];
                }

                idxStartPos = pos + 1;

                FindSignatures(segmentBody, idxStartPos, idxEndPos);
            }

            // Define the body tags
            foreach(var line in segmentBody)
            {
                if( line.Tag == TaggedSegmentEnum.None )
                {
                    if (line.TextAlignment == TextAlignment.CENTER)
                        line.Tag = TaggedSegmentEnum.Subtitulo;
                }
            }

            return taggedSegment;
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


        bool IsDate(string body)
        {
            var match = Regex.Match(body, @"\d+\s+de\s+[a-zA-Z]+\s+de\s+\d\d\d\d");

            return (match.Success);
        }

        void FindSignatures(IEnumerable<TextTaggedStructure> bodyEnum, int start, int end)
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

        void ProcessSignatures(IEnumerable<TextTaggedStructure> bodyEnum)
        {
            foreach(var b in bodyEnum)
            {
                string text = b.TextStructure.Text;

                // exception: italic = name
                bool isItalic = b.TextStructure.FontStyle.Contains("Italic");

                b.Tag = (IsOneLineUpperCase(text) || isItalic) ? TaggedSegmentEnum.Assinatura : TaggedSegmentEnum.Cargo;
            }

            foreach (var b in bodyEnum)
            {
                string text = b.TextStructure.Text;
                bool singleLine = text.Contains("\n");

                if (IsDate(text) && singleLine)
                    b.Tag = TaggedSegmentEnum.Data;
            }
        }
        
        bool IsUpperCase(string text)
        {
            return text == text.ToUpper();
        }

        bool IsOneLineUpperCase(string text)
        {
            string[] multipleLines = text.Split('\n');

            foreach(var line in multipleLines)
            {
                if (IsUpperCase(line))
                    return true;
            }
            return false;
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
