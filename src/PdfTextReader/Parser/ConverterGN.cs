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
        public string Convert(string pdf, string article, string content)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<xml>" + content + "</xml>");

            var xmlArticle = doc.GetElementsByTagName("article")[0];

            var artPubName = doc.CreateAttribute("pubName");
            artPubName.Value = GetPubName(pdf);

            var artArtType = doc.CreateAttribute("artType");
            artArtType.Value = GetArtType();

            var artPubDate = doc.CreateAttribute("pubDate");
            artPubDate.Value = GetPubDate(pdf);

            var artArtCategory = doc.CreateAttribute("artCategory");
            artArtCategory.Value = GetArtCategory(doc);

            //var artNumberPage = doc.CreateAttribute("numberPage");
            //artNumberPage.Value = GetNumberPage(doc);

            string pubdate = GetPubDate(pdf);
            string numberPage = GetNumberPage(doc);

            var artPdfPage = doc.CreateAttribute("pdfPage");
            artPdfPage.Value = GetPdfPage(pubdate, "515", numberPage);

            var artEditionNumber = doc.CreateAttribute("editionNumber");
            artEditionNumber.Value = GetEditionNumber();
            
            xmlArticle.Attributes.Append(artPubName);
            xmlArticle.Attributes.Append(artArtType);
            xmlArticle.Attributes.Append(artPubDate);
            xmlArticle.Attributes.Append(artArtCategory);
            xmlArticle.Attributes.Append(artPdfPage);
            xmlArticle.Attributes.Append(artEditionNumber);

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

        string GenerateArticleId()
        {
            return "1";
        }

        string GetPubName(string pdf)
        {
            string[] comps = pdf.Split('_');
            return comps[0];
        }

        string GetPubDate(string pdf)
        {
            string[] comps = pdf.Split('_');
            string year = comps[1];
            string month = comps[2];
            string day = comps[3];
            return $"{day}/{month}/{year}";
        }

        string GetArtType()
        {
            return "?";
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

        string GetPdfPage(string pubdate, string jornal, string pagina)
        {
            string baseUrl = "http://pesquisa.in.gov.br/imprensa/jsp/visualiza/index.jsp";
            string query = $"?data={pubdate}&amp;jornal={jornal}&amp;pagina={pagina}";

            return baseUrl + query;
        }

        string GetEditionNumber()
        {
            return "?";
        }

        XmlNode GetBodyTexto(string docname, XmlDocument doc)
        {
            var text = doc.SelectSingleNode("xml/article/body/Texto")
                .InnerText
                //[[[IMG(page=1,50.3414,39.14461,275.953,133.7223)]]]
                //.Replace("/api/images/DO1_2010_01_04/parser/IMG(page=1,50.3414,39.14461,275.953,133.7223)")
                
                .Replace("[[[", $"\n<img src='/images/{docname}/").Replace("]]]", "'>\n")
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
