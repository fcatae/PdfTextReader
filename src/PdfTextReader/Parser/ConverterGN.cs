using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Linq;
using System.IO;

namespace PdfTextReader.Parser
{
    class ConverterGN
    {
        Jornal _jornal;

        public string Convert(string pdf, string article, string content)
        {
            _jornal = new Jornal(pdf);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<xml>" + content + "</xml>");

            var xmlArticle = doc.GetElementsByTagName("article")[0];

            //var artNumberPage = doc.CreateAttribute("numberPage");
            //artNumberPage.Value = GetNumberPage(doc);

            string numberPage = GetNumberPage(doc);
            
            AddAttribute(xmlArticle, "pubName", _jornal.PubName);
            AddAttribute(xmlArticle, "artType", GetArtType());
            AddAttribute(xmlArticle, "pubDate", _jornal.PubDate);
            AddAttribute(xmlArticle, "artCategory", GetArtCategory(doc));
            AddAttribute(xmlArticle, "pdfPage", _jornal.GetDocumentPageUrl(numberPage));
            AddAttribute(xmlArticle, "editionNumber", _jornal.GetEditionNumber());

            var xmlBody = xmlArticle.SelectSingleNode("body");

            var xmlTexto = xmlBody.SelectSingleNode("Texto");
            var bodyTexto = GetBodyTexto(pdf, doc);
            xmlTexto.InnerXml = bodyTexto.OuterXml;

            var bodyHierarquia = xmlBody.SelectSingleNode("Hierarquia");
            xmlBody.RemoveChild(bodyHierarquia);

            var bodyArtigo = xmlBody.SelectSingleNode("Artigo");
            xmlBody.RemoveChild(bodyArtigo);

            var xmlIdentifica = xmlBody.SelectSingleNode("Identifica");
            var xmlData = doc.CreateElement("Data");
            xmlBody.InsertAfter(xmlData, xmlIdentifica);

            string output = GetPrettyXml(doc);

            return output;
        }

        void AddAttribute(XmlNode xmlArticle, string attributeName, string value)
        {
            if (value == null)
                return;

            var doc = xmlArticle.OwnerDocument;
            var attrib = doc.CreateAttribute(attributeName);
            attrib.Value = value;

            xmlArticle.Attributes.Append(attrib);
        }

        string GetArtType()
        {
            return null;
        }

        string GetArtCategory(XmlDocument doc)
        {
            var nodes = doc.SelectNodes("xml/article/body/Hierarquia/Classe").Cast<XmlNode>();
            var hierarquiaClasses = nodes.Select(x => x.InnerText.Replace("/", "")).ToList();
            hierarquiaClasses.RemoveAt(hierarquiaClasses.Count - 1);
            return String.Join("/", hierarquiaClasses);
        }

        string GetNumberPage(XmlDocument doc)
        {
            return doc.SelectSingleNode("xml/article/@numberPage").Value;
        }
        
        XmlNode GetBodyTexto(string docname, XmlDocument doc)
        {
            var text = doc.SelectSingleNode("xml/article/body/Texto")
                .InnerText
                //[[[IMG(page=1,50.3414,39.14461,275.953,133.7223)]]]
                //.Replace("/api/images/DO1_2010_01_04/parser/IMG(page=1,50.3414,39.14461,275.953,133.7223)")
                
                .Replace("[[[", $"</p>\n<p class='image'><img class='pdf' src='/images/{docname}/").Replace("]]]", "'></p>\n<p>")
                .Replace("</p>","</p>\n");
            
            var identifica = doc.SelectSingleNode("xml/article/body/Identifica");
            var autores = doc.SelectSingleNode("xml/article/body/Autores");

            var assinaturas = (autores != null) ? autores
                                .ChildNodes
                                .Cast<XmlNode>()
                                .Where(x => !String.IsNullOrEmpty(x.InnerText))
                                .Select(x => $"<p class='{x.Name}'>{x.InnerText}</p>")
                                .ToArray() :
                                new string[] { };
                                
            string newbody = $"<p class=\"identifica\">{identifica.InnerText}</p>\n{text}\n{String.Join("\n",assinaturas)}";

            return doc.CreateCDataSection(newbody);
        }

        string GetPrettyXml(XmlDocument doc)
        {
            var settings = new XmlWriterSettings()
            {
                OmitXmlDeclaration = true,
                Indent = true
            };

            var stringWriter = new StringWriter();

            using (XmlWriter writer = XmlWriter.Create(stringWriter, settings))
            {
                doc.WriteTo(writer);
            }

            return stringWriter.ToString();
        }
    }
}
