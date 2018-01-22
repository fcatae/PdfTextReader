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
        public void XMLWriter(IEnumerable<Artigo> artigos, string doc)
        {
            var settings = new XmlWriterSettings()
            {
                Indent = true                
            };
            using (XmlWriter writer = XmlWriter.Create($"{doc}.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Artigo");

                foreach (Artigo artigo in artigos)
                {
                    Conteudo conteudo = artigo.Conteudo;
                    Metadados metadados = artigo.Metadados;


                    //Writing Metadata
                    writer.WriteStartElement("Metadados");

                    writer.WriteAttributeString("ID", conteudo.IntenalId.ToString());
                    if (metadados.Nome != null)
                        writer.WriteAttributeString("Nome", ConvertBreakline2Space(metadados.Nome));
                    if (metadados.TipoDoArtigo != null)
                        writer.WriteAttributeString("TipoDoArtigo", ConvertBreakline2Space(metadados.TipoDoArtigo));
                    if (conteudo.Hierarquia != null)
                        writer.WriteAttributeString("Hierarquia", ConvertBreakline2Space(conteudo.Hierarquia));
                    if (metadados.Grade != null)
                        writer.WriteAttributeString("Grade", ConvertBreakline2Space(metadados.Grade));

                    writer.WriteEndElement();

                    //Writing Body
                    writer.WriteStartElement("Conteudo");

                    if (conteudo.Titulo != null)
                        writer.WriteElementString("Titulo", ConvertBreakline2Space(conteudo.Titulo));
                    if (conteudo.Caput != null)
                        writer.WriteElementString("Caput", conteudo.Caput);
                    if (conteudo.Corpo != null)
                        writer.WriteElementString("Corpo", conteudo.Corpo);
                    if (conteudo.Assinatura != null)
                    {
                        writer.WriteStartElement("Autores");
                        foreach (var ass in conteudo.Assinatura)
                        {
                            if (ass.Length > 3)
                                writer.WriteElementString("Assinatura", ass);
                        }
                        writer.WriteEndElement();
                    }
                    if (conteudo.Cargo != null)
                        writer.WriteElementString("Cargo", ConvertBreakline2Space(conteudo.Cargo));
                    if (conteudo.Data != null)
                        writer.WriteElementString("Data", conteudo.Data);

                    writer.WriteEndElement();

                    //Writing Anexo
                    writer.WriteStartElement("Anexo");
                    if (conteudo.Anexo.HierarquiaTitulo != null)
                        writer.WriteElementString("Hierarquia", conteudo.Anexo.HierarquiaTitulo);
                    if (conteudo.Anexo.Titulo != null)
                        writer.WriteElementString("Titulo Anexo", conteudo.Anexo.Titulo);
                    if (conteudo.Anexo.Texto != null)
                        writer.WriteElementString("Texto", conteudo.Anexo.Texto);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        string ConvertBreakline2Space(string input)
        {
            string output = input.Replace("\n", " ");
            if (output.Contains(":"))
            {
                output = output.Substring(0, output.Length - 1);
            }
            return output;
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
