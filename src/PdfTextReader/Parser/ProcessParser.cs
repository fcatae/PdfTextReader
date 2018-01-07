using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace PdfTextReader.Parser
{
    class ProcessParser
    {
        decimal Tolerance = 3;
        public List<Conteudo> ProcessStructures(IEnumerable<Structure.TextStructure> structures)
        {
            List<Conteudo> contents = new List<Conteudo>();
            foreach (Structure.TextStructure structure in structures)
            {
                if (structure.CountLines() == 1 && structure.TextAlignment == Structure.TextAlignment.RIGHT && structure.MarginRight > Tolerance && structure.Text.ToUpper() == structure.Text)
                {
                    contents.Add(new Conteudo(structure, TipoDoConteudo.Assinatura));
                }
                else if (structure.CountLines() == 1 && structure.TextAlignment == Structure.TextAlignment.RIGHT && structure.MarginRight > Tolerance)
                {
                    contents.Add(new Conteudo(structure, TipoDoConteudo.Cargo));
                }
                else if (structure.CountLines() > 1 && structure.TextAlignment == Structure.TextAlignment.JUSTIFY)
                {
                    contents.Add(new Conteudo(structure, TipoDoConteudo.Corpo));
                }
                else if (structure.CountLines() == 1 && structure.TextAlignment == Structure.TextAlignment.RIGHT && structure.MarginRight < Tolerance)
                {
                    contents.Add(new Conteudo(structure, TipoDoConteudo.Caput));
                }
                else if (structure.TextAlignment == Structure.TextAlignment.CENTER && structure.FontStyle == "Bold")
                {
                    if (Stats.ProcessStats.GetGridStyle() != null && structure.FontName == Stats.ProcessStats.GetGridStyle().FontName)
                    {
                        contents.Add(new Conteudo(structure, TipoDoConteudo.Grade));
                    }
                    else if (structure.FontSize > 9) // Preciso pegar do Stats
                    {
                        contents.Add(new Conteudo(structure, TipoDoConteudo.Setor));
                    }
                    else
                    {
                        contents.Add(new Conteudo(structure, TipoDoConteudo.Título));
                    }
                }
                else if (structure.TextAlignment == Structure.TextAlignment.CENTER && structure.Text.ToUpper() != structure.Text)
                {
                    contents.Add(new Conteudo(structure, TipoDoConteudo.Data));
                }
                else if (structure.TextAlignment == Structure.TextAlignment.CENTER)
                {
                    contents.Add(new Conteudo(structure, TipoDoConteudo.Departamento));
                }
            }
            return contents;
        }

        public void XMLWriter(IEnumerable<Artigo> artigos, string doc)
        {
            using (XmlWriter writer = XmlWriter.Create($"{doc}.xml"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Página");

                foreach (Artigo artigo in artigos)
                {
                    writer.WriteStartElement("Artigo");

                    writer.WriteElementString("Título", artigo.Titulo);
                    writer.WriteElementString("Corpo", artigo.Corpo);
                    writer.WriteElementString("Assinatura", artigo.Assinatura);
                    writer.WriteElementString("Cargo", artigo.Cargo);

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        public IEnumerable<Artigo> BuildArticles(List<Conteudo> conteudos)
        {
            List<Artigo> artigos = new List<Artigo>();


            int CutPosition = conteudos.IndexOf(conteudos.Where(c => c.ContentType == TipoDoConteudo.Setor).ToList().FirstOrDefault());

            List<Conteudo> p1 = conteudos.GetRange(0, CutPosition - 1);
            List<Conteudo> p2 = conteudos.GetRange(CutPosition, conteudos.Count - 1);

            {//FORÇADO
             //No caso p1 a lista se divide pela data

                int cutDate = p1.IndexOf(p1.Where(p => p.ContentType == TipoDoConteudo.Data).ToList().FirstOrDefault());

                List<Conteudo> pp1 = p1.GetRange(0, cutDate - 1);
                List<Conteudo> pp2 = p1.GetRange(cutDate, p1.Count - 1);

            }


            return artigos;
        }
    }
}
