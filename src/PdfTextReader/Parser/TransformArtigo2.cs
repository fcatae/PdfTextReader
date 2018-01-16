using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfTextReader.Parser
{
    class TransformArtigo2
    {
        public IEnumerable<Artigo> Create(IList<Conteudo> conteudos, string basename)
        {

            List<Artigo> artigos = new List<Artigo>();

            foreach (Conteudo conteudo in conteudos)
            {

                Metadados metadados = new Metadados()
                {
                   
                };


                var article = new Artigo
                {
                    Metadados = metadados,
                    Conteudo = conteudo
                };

                artigos.Add(article);
            }

            return artigos;
        }

        private string GetGrade(string input)
        {
            return null;
        }

        public void CreateXML(IEnumerable<Artigo> artigos, string basename)
        {
            var procParser = new ProcessParser();
            procParser.XMLWriterMultiple(artigos, $"bin/{basename}-artigo");
        }
    }
}