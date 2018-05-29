using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PdfTextReader.Parser
{
    class TransformArtigo2 : IAggregateStructure<Conteudo, Artigo>
    {
        bool ContainsExclusiveTitles(string text)
        {
            string[] list =
                {
                "sumario",
                "sumário"
            };

            if (text != null)
            {
                foreach (string s in list)
                {
                    if (text.ToLower().Contains(s))
                        return true;
                }
            }

            return false;
        }

        private string GetGrade(string input)
        {
            string output = null;
            if (input != null && input.Contains(":"))
            {
                int idxGrade = input.IndexOf(":", 0);
                output = input.Substring(0, idxGrade);
            }

            return output;
        }

        private string GetName(string input)
        {
            string output = null;
            if (input != null)
            {
                string p1 = input.Split(',')[0];
                //Get Type
                output = GetType(input);

                //Get Number
                if (p1 != null)
                {
                    Regex number = new Regex(@"([0-9]+\.?(\/)?[0-9]*)", RegexOptions.CultureInvariant);
                    Match mNumber = number.Match(input);
                    if (mNumber.Success)
                        output = output + $" {mNumber.Groups[0].ToString()}";
                }

                //Get Year
                Regex r = new Regex(@"(\d{4})", RegexOptions.CultureInvariant);
                Match m = r.Match(input);
                if (m.Success)
                    output = output + $"-{m.Groups[0].ToString()}";
            }

            return output;
        }

        private string GetIdMateria(string title)
        {
            return TitleWithHiddenIdMateria.GetIdMateria(title);
        }

        private string CleanIdMateria(string title)
        {
            return TitleWithHiddenIdMateria.CleanIdMateria(title);
        }

        private string GetType(string input)
        {
            string output = null;

            if (input != null)
            {
                if (input.Contains("No") || input.Contains("N°") || input.Contains("Nº"))
                {
                    Regex number = new Regex(@"(.+?(?= N.))", RegexOptions.CultureInvariant);
                    Match mNumber = number.Match(input);
                    if (mNumber.Success)
                        output = mNumber.Groups[0].ToString();
                }
                else if (input.Contains("DE"))
                {
                    Regex number = new Regex(@"(.+?((?= DE)|(?= DA)))", RegexOptions.CultureInvariant);
                    Match mNumber = number.Match(input);
                    if (mNumber.Success)
                        output = mNumber.Groups[0].ToString();
                }
                else
                {
                    output = input;
                }
            }

            return output;
        }
        
        public void CreateXML(IEnumerable<Artigo> artigos, string outputfolder, string basename)
        {
            var procParser = new ProcessParser();
            procParser.XMLWriterMultiple(artigos, $"{outputfolder}/{basename}-artigo");
        }

        public void CreateJson(IEnumerable<Artigo> artigos, string outputfolder, string basename)
        {
            var procParser = new ProcessParserJson();
            procParser.WriteJson(artigos, $"{outputfolder}/{basename}-artigo");
        }

        public void Init(Conteudo line)
        {            
        }

        public bool Aggregate(Conteudo line)
        {
            return false;
        }

        public Artigo Create(List<Conteudo> input)
        {
            var conteudo = input[0];

            string titulo = CleanIdMateria(conteudo.Titulo);
            Metadados metadados = new Metadados()
            {
                TipoDoArtigo = GetType(titulo),
                Nome = GetName(titulo),
                Grade = GetGrade(conteudo.Hierarquia),
                NumeroDaPagina = conteudo.Page,
                Titulo = titulo,
                IdMateria = GetIdMateria(conteudo.Titulo)
            };


            var article = new Artigo
            {
                Metadados = metadados,
                Conteudo = conteudo,
                Anexos = conteudo.Anexos
            };

            return article;
        }
    }
}