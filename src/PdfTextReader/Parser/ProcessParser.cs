using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using PdfTextReader.Base;

namespace PdfTextReader.Parser
{
    class ProcessParser
    {
        float Tolerance = 3;
        public List<Conteudo> ProcessStructures(IEnumerable<TextStructure> structures)
        {
            List<Conteudo> contents = new List<Conteudo>();
            foreach (TextStructure structure in structures)
            {
                if (structure.CountLines() == 1 && structure.TextAlignment == TextAlignment.RIGHT && structure.MarginRight > Tolerance && structure.Text.ToUpper() == structure.Text)
                {
                    contents.Add(new Conteudo(structure, TipoDoConteudo.Assinatura));
                }
                else if (structure.CountLines() == 1 && structure.TextAlignment == TextAlignment.RIGHT && structure.MarginRight > Tolerance)
                {
                    contents.Add(new Conteudo(structure, TipoDoConteudo.Cargo));
                }
                else if (structure.CountLines() > 1 && structure.TextAlignment == TextAlignment.JUSTIFY)
                {
                    contents.Add(new Conteudo(structure, TipoDoConteudo.Corpo));
                }
                else if (structure.CountLines() == 1 && structure.TextAlignment == TextAlignment.RIGHT && structure.MarginRight < Tolerance)
                {
                    contents.Add(new Conteudo(structure, TipoDoConteudo.Caput));
                }
                else if (structure.TextAlignment == TextAlignment.CENTER && structure.FontStyle == "Bold")
                {
                    if (ExecutionStats.ProcessStats.GetGridStyle() != null && structure.FontName == ExecutionStats.ProcessStats.GetGridStyle().FontName)
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
                else if (structure.TextAlignment == TextAlignment.CENTER && structure.Text.ToUpper() != structure.Text)
                {
                    contents.Add(new Conteudo(structure, TipoDoConteudo.Data));
                }
                else if (structure.TextAlignment == TextAlignment.CENTER)
                {
                    contents.Add(new Conteudo(structure, TipoDoConteudo.Departamento));
                }
            }
            return contents;
        }

        public void XMLWriter(IEnumerable<Artigo> artigos, string doc)
        {
            var settings = new XmlWriterSettings()
            {
                Indent = true                
            };
            using (XmlWriter writer = XmlWriter.Create($"{doc}.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Metadados");

                foreach (Artigo artigo in artigos)
                {
                    if (artigo.Hierarquia != null)
                        writer.WriteAttributeString("Hierarquia", ConvertBreakline2Space(artigo.Hierarquia));

                    writer.WriteStartElement("Conteudo");

                    if (artigo.Titulo != null)
                        writer.WriteElementString("Titulo", artigo.Titulo);
                    if (artigo.Caput != null)
                        writer.WriteElementString("Caput", artigo.Caput);
                    if (artigo.Corpo != null)
                        writer.WriteElementString("Corpo", artigo.Corpo);
                    if (artigo.Assinatura != null)
                        writer.WriteElementString("Assinatura", artigo.Assinatura);
                    if (artigo.Cargo != null)
                        writer.WriteElementString("Cargo", artigo.Cargo);
                    if (artigo.Data != null)
                        writer.WriteElementString("Data", artigo.Data);

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        string ConvertBreakline2Space(string input)
        {
            return input.Replace("\n", " ").Substring(0,input.Length - 1);
        }
        public void XMLWriterMultiple(IEnumerable<Artigo> artigos, string doc)
        {
            int i = 1;
            foreach(var artigo in artigos)
            {
                string doc_i = doc + (i++);
                var artigo_i = new Artigo[] { artigo };

                this.XMLWriter(artigo_i, doc_i);
            }
        }
    }
}
