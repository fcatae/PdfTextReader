using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using PdfTextReader.Base;

namespace PdfTextReader.Parser
{
    class ProcessParser
    {
        int countPerPage = 1;
        string initialPage = "0001";
        public void XMLWriter(IEnumerable<Artigo> artigos, string doc)
        {
            string finalURL = doc;

            var settings = new XmlWriterSettings()
            {
                OmitXmlDeclaration = true, // omit XML declaration
                Indent = true                
            };

            using (Stream virtualStream = VirtualFS.OpenWrite($"{finalURL}.xml"))
            using (XmlWriter writer = XmlWriter.Create(virtualStream, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("article");

                foreach (Artigo artigo in artigos)
                {
                    Conteudo conteudo = artigo.Conteudo;
                    Metadados metadados = artigo.Metadados;
                    List<Anexo> anexos = artigo.Anexos;

                    if (metadados.IdMateria!= null)
                        writer.WriteAttributeString("idmateria", metadados.IdMateria);

                    //if (conteudo.Hierarquia != null)
                    //    writer.WriteAttributeString("hierarquia", ConvertBreakline2Space(conteudo.Hierarquia));
                    
                    //writer.WriteAttributeString("artSection", ConvertBreakline2Space(metadados.Grade));
                    
                    writer.WriteAttributeString("numberPage", metadados.NumeroDaPagina.ToString());

                    writer.WriteStartElement("body");

                    // Hierarquia
                    writer.WriteStartElement("Hierarquia");
                    foreach(var classe in conteudo.HierarquiaTitulo)
                    {
                        writer.WriteElementString("Classe", classe);
                    }
                    
                    writer.WriteEndElement();

                    // Artigo
                    writer.WriteStartElement("Artigo");
                    writer.WriteCData("\n" + conteudo.Texto + "\n");
                    writer.WriteEndElement();

                    //Writing Body
                    writer.WriteElementString("Identifica", ConvertBreakline2Space(metadados.Titulo));
                    writer.WriteElementString("Ementa", conteudo.Caput);

                    writer.WriteStartElement("Texto");
                    writer.WriteCData(conteudo.Corpo);
                    writer.WriteEndElement();

                    if (conteudo.Autor.Count > 0)
                    {
                        writer.WriteStartElement("Autores");

                        foreach (Autor autor in conteudo.Autor)
                        {
                            writer.WriteElementString("assina", ConvertBreakline2Space(autor.Assinatura));
                            if (autor.Cargo != null)
                            {
                                writer.WriteElementString("cargo", ConvertBreakline2Space(autor.Cargo));
                            }
                        }

                        writer.WriteEndElement();
                    }

                    if (conteudo.Data != null)
                        writer.WriteElementString("Data", conteudo.Data);

                    writer.WriteEndElement();

                    //Writing Anexos
                    if (anexos.Count > 0)
                    {
                        writer.WriteStartElement("Anexos");
                        foreach (Anexo item in anexos)
                        {
                            writer.WriteStartElement("Anexo");
                            if (item.HierarquiaTitulo != null)
                                writer.WriteElementString("Hierarquia", item.HierarquiaTitulo);
                            if (item.Titulo != null)
                                writer.WriteElementString("Titulo", item.Titulo);
                            if (item.Texto != null)
                                writer.WriteElementString("Texto", item.Texto);
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                    }
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();

            }
        }

        private string ProcessName(Artigo artigo, string doc)
        {
            string numpag = artigo.Metadados.NumeroDaPagina.ToString().PadLeft(4, '0');
            string docFinalText = new DirectoryInfo(doc).Name;
            string date = docFinalText.Substring(4, 10).Replace("_", "");
            string globalId = docFinalText.Substring(21, docFinalText.Length - 21);
            string modelNameGlobal = $"{date}-{globalId}";
            string modelNameCustom = null;

            if (numpag == initialPage)
            {
                modelNameCustom = $"{date}-{numpag}-{countPerPage++.ToString().PadLeft(2, '0')}";
            }
            else
            {
                initialPage = numpag;
                countPerPage = 1;
                modelNameCustom = $"{date}-{numpag}-{countPerPage.ToString().PadLeft(2, '0')}";
            }


            return doc.Replace(docFinalText, modelNameCustom);
        }

        string ConvertBreakline2Space(string input)
        {
            if (input == null)
                return null;

            string output = input;
            if (input != null && input.Length > 1)
            {
                output = input.Replace("\n", " ");
                if (output.Contains(":"))
                {
                    output = output.Substring(0, output.Length - 1);
                }
                if (output.Substring(0, 1) == " ")
                    output = output.Substring(1, output.Length - 1);
            }
            return output;
        }

        public void XMLWriterMultiple(IEnumerable<Artigo> artigos, string doc)
        {
            int i = 0;
            foreach(var artigo in artigos)
            {
                string doc_i = doc + (i++);
                var artigo_i = new Artigo[] { artigo };

                this.XMLWriter(artigo_i, doc_i);
            }
        }
    }
}
