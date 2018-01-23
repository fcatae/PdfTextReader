using PdfTextReader.TextStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;
using System.Text.RegularExpressions;

namespace PdfTextReader.Parser
{
    class TransformConteudo : IAggregateStructure<TextSegment, Conteudo>
    {
        int count = 0;
        public bool Aggregate(TextSegment line)
        {
            return false;
        }

        public Conteudo Create(List<TextSegment> segments)
        {
            count++;
            TextSegment segment = ProcessSingularBodyBehaviors(segments[0]);

            ProcessExclusiveText(segment.Body);

            int page = segment.Body[0].Lines[0].PageInfo.PageNumber;

            string titulo = null;
            string hierarchy = null;
            string body = null;
            string caput = null;
            string possibleData = null;
            List<Autor> autores = null;
            string data = null;
            List<Anexo> anexos = null;
            //just for process results
            List<string> resultProcess = new List<string>() { null, null, null };


            //Definindo Titulo e hierarquia
            int idxTitle = segment.Title.Count() - 1;

            if (idxTitle == 0)
            {
                titulo = segment.Title[0].Text;
            }
            else if (idxTitle > 0)
            {
                for (int i = 0; i < segment.Title.Count() - 1; i++)
                {
                    hierarchy = hierarchy + segment.Title[i].Text + ":";
                }
                titulo = ProcessSingularTitles(segment.Title, segment.Title[idxTitle].Text);
            }

            //Definindo Caput
            if (segment.Body.Count() > 1)
            {
                if (segment.Body[0].TextAlignment == TextAlignment.RIGHT && segment.Body[1].TextAlignment == TextAlignment.JUSTIFY)
                    caput = segment.Body[0].Text;
            }


            //Definindo Assinatura, Cargo e Data
            int idxSigna = 0;
            //Se contiver anexo...
            if (caput != null)
            {
                idxSigna = segment.Body.ToList().FindIndex(2, s => s.TextAlignment == TextAlignment.RIGHT);
            }
            else //Caso não tenha anexo
            {
                idxSigna = segment.Body.ToList().FindLastIndex(s => s.TextAlignment == TextAlignment.JUSTIFY) + 1;
            }

            autores = new List<Autor>();
            if (idxSigna > 0 && idxSigna < segment.Body.Count())
            {
                resultProcess.Clear();
                resultProcess = ProcessSignatureAndRole(segment.Body[idxSigna].Lines);
                autores.Add(new Autor() { Assinatura = resultProcess[0], Cargo = resultProcess[1] });  
                data = resultProcess[2];
            }

            //Definindo Body
            if (caput != null && idxSigna > 0 && idxSigna < segment.Body.Count())
            {
                body = String.Join("\n", segment.Body.Skip(1).Take(idxSigna - 1).Select(s => s.Text));
                if (idxSigna > 0 && idxSigna < segment.Body.Count())
                    possibleData = segment.Body[idxSigna - 1].Text;
            }
            else if (idxSigna > 0 && idxSigna < segment.Body.Count())
            {
                var valueToTake = idxSigna - 1;
                if (valueToTake == 0)
                    valueToTake = 1;
                body = String.Join("\n", segment.Body.Take(valueToTake).Select(s => s.Text));
                if (idxSigna > 1 && idxSigna < segment.Body.Count())
                    possibleData = segment.Body[idxSigna - 1].Text;
            }
            else
            {
                if (caput != null)
                {
                    body = String.Join("\n", segment.Body.Skip(1).Take(segment.Body.Count()).Select(s => s.Text));
                    possibleData = segment.Body[segment.Body.Count() - 1].Text;
                }
                else
                {
                    body = String.Join("\n", segment.Body.Take(segment.Body.Count()).Select(s => s.Text));
                    possibleData = segment.Body[segment.Body.Count() - 1].Text;
                }
            }

            //Definindo o Anexo se existir e verificando se necessita juntar as assinaturas
            var resultSignAndAnexo = ProcessAnexoOrSign(segment.Body, idxSigna);
            anexos = new List<Anexo>();
            if (resultSignAndAnexo[1].Count() > 0)
            {
                anexos.Add(ConcatAnexo(resultSignAndAnexo[1]));
            }

            if (resultSignAndAnexo[0].Count() > 0)
            {
                var result = ProcessListOfSignatures(resultSignAndAnexo[0].ToList());
                foreach (Autor item in result)
                {
                    autores.Add(item);
                }
            }


            //Verificando se Data ficou na assinatura
            if (data == null)
                if (possibleData != null)
                    data = HasData(possibleData);
            if (data != null)
                body = RemoveDataFromBody(body, data);

            return new Conteudo()
            {
                IntenalId = count,
                Page = page,
                Hierarquia = hierarchy,
                Titulo = titulo,
                Caput = caput,
                Corpo = body,
                Autor = autores,
                Data = data,
                Anexos = anexos
            };
        }

        string RemoveDataFromBody(string body, string data)
        {
            return body.Replace(data, "");
        }

        string HasData(string body)
        {
            string result = null;

            var match = Regex.Match(body, @"(.+?[a-zA-Z]+, \d\d de [a-zA-Z]+ de \d{4})");

            if (match.Success)
                return body;

            return result;
        }

        List<TextStructure[]> ProcessAnexoOrSign(TextStructure[] structures, int idxSigna)
        {
            List<TextStructure> sign = new List<TextStructure>();
            List<TextStructure> anexo = new List<TextStructure>();
            IEnumerable<TextStructure> discover;

            if (idxSigna > 0 && structures.Count() > idxSigna)
            {
                discover = structures.Skip(idxSigna + 1).Take(structures.Count() - idxSigna);

                foreach (var item in discover)
                {
                    if (item.TextAlignment == TextAlignment.JUSTIFY)
                    {
                        anexo.Add(item);
                    }
                    else
                    {
                        sign.Add(item);
                    }
                }
            }
            return new List<TextStructure[]>() { sign.ToArray(), anexo.ToArray() };
        }

        void ProcessExclusiveText(TextStructure[] structures)
        {
            foreach (TextStructure item in structures)
            {
                if (item.Text.ToLower() == "o presidente da república" || item.Text.ToLower() == "a presidenta da república")
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
                else if (item.Text.All(Char.IsDigit))
                {
                    date = item.Text;
                }
                else
                {
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

        string ProcessSingularTitles(TextStructure[] segmentTitles, string title)
        {
            string newTitle = null;

            //If title was single data (e.g. "Em 25 de Dezembro de 2016")
            var match = Regex.Match(title, @"((Em|EM|DIA|dia) [0-9]*(\°?|o?|\º?) (de|DE) [a-zA-Z]+ (de|DE) \d{4})");
            //Get the last position before title and concat with it.
            if (match.Success)
                newTitle = $"{segmentTitles[segmentTitles.Count() - 2].Text} - {title.ToUpper()}";


            //If title is specific like "Relação N°"
            var match2 = Regex.Match(title, @"(RELAÇÃO (No|N°|Nº)\-? [0-9]*\/[0-9]*)");
            //Get the last position before title and concat with it.
            if (match2.Success)
                newTitle = $"{segmentTitles[segmentTitles.Count() - 2].Text} - {title.ToUpper()}";

            if (newTitle == null)
            {
                return title;
            }
            else
            {
                return newTitle;
            }
        }

        TextSegment ProcessSingularBodyBehaviors(TextSegment segment)
        {
            var match = Regex.Match(segment.Body[0].Text, @"(.*? (No|N°|Nº)\-? ([0-9]+\.?(\/)?[0-9]*), [a-zA-Z]* [0-9]* [a-zA-Z]* [a-zA-Z]* [a-zA-Z]* [0-9]*)");

            if (match.Success)
            {
                int TOLERANCE_PERIOD_CHAR = 2;
                if (segment.Body[0].Text.Length - match.Length < TOLERANCE_PERIOD_CHAR)
                {
                    segment.Body[0].TextAlignment = TextAlignment.CENTER;
                    List<TextStructure> newTitle = segment.Title.ToList();
                    newTitle.Add(segment.Body[0]);
                    segment.Title = newTitle.ToArray();
                    segment.Body = segment.Body.Where(b => b != segment.Body[0]).ToArray();
                }
            }


            var match2 = Regex.Match(segment.Body[0].Text, @"(.*? [a-zA-Z]* [0-9]* [a-zA-Z]* [a-zA-Z]* [a-zA-Z]* [0-9]*)");

            if (match2.Success)
            {
                if (match2.Length == segment.Body[0].Text.Length)
                {
                    segment.Body[0].TextAlignment = TextAlignment.CENTER;
                    List<TextStructure> newTitle = segment.Title.ToList();
                    newTitle.Add(segment.Body[0]);
                    segment.Title = newTitle.ToArray();
                    segment.Body = segment.Body.Where(b => b != segment.Body[0]).ToArray();
                }
            }

            //If doc is PAUTA DE JULGAMENTO
            if (segment.Body[0].Text.Count() > 20)
            {
                if (segment.Body[0].Text.Substring(0, 19) == "PAUTA DE JULGAMENTO")
                {
                    segment.Body[0].TextAlignment = TextAlignment.CENTER;
                    List<TextStructure> newTitle = segment.Title.ToList();
                    newTitle.Add(segment.Body[0]);
                    segment.Title = newTitle.ToArray();
                    segment.Body = segment.Body.Where(b => b != segment.Body[0]).ToArray();
                }
            }

            return segment;
        }

        Anexo ConcatAnexo(TextStructure[] structures)
        {
            return new Anexo(String.Join("\n", structures.Take(structures.Count()).Select(t => t.Text))) { };
        }

        public void Init(TextSegment line)
        {

        }
    }
}
