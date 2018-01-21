using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PdfTextReader.Parser
{
    class TransformArtigo2
    {
        public IEnumerable<Artigo> Create(IList<Conteudo> conteudos)
        {

            List<Artigo> artigos = new List<Artigo>();

            foreach (Conteudo conteudo in conteudos)
            {

                Metadados metadados = new Metadados()
                {
                    TipoDoArtigo = GetType(conteudo.Titulo),
                    Nome = GetName(conteudo.Titulo),
                    Grade = GetGrade(conteudo.Hierarquia)
                };


                var article = new Artigo
                {
                    Metadados = metadados,
                    Conteudo = conteudo
                };

                if (IsArticle(article))
                    artigos.Add(article);
            }

            return artigos;
        }

        bool IsArticle(Artigo article)
        {
            if (IsNullOrPeriod(article.Conteudo.Corpo))
                return false;
            if (ContainsExclusiveTitles(article.Conteudo.Titulo))
                return false;
            return true;
        }

        bool IsNullOrPeriod(string text)
        {
            if (text == null || text == ".")
                return true;
            return false;
        }

        bool ContainsExclusiveTitles(string text)
        {
            string[] list =
                {
                "sumario",
                "sumário"
            };

            foreach (string s in list)
            {
                if (text.ToLower().Contains(s))
                    return true;
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
                string p1 = input.Split(",")[0];
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

        private string GetType(string input)
        {
            string output = null;

            if (input != null)
            {
                if (input.Contains("No") || input.Contains("N°"))
                {
                    Regex number = new Regex(@"(.+?(?= No)|(?= N°))", RegexOptions.CultureInvariant);
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

        public void CreateXML(IEnumerable<Artigo> artigos, string basename)
        {
            var procParser = new ProcessParser();
            procParser.XMLWriterMultiple(artigos, $"bin/{basename}-artigo");
        }
    }
}