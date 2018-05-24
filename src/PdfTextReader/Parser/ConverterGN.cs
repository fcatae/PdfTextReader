using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Linq;

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
            var bodyHierarquia = xmlBody.SelectSingleNode("Hierarquia");
            xmlBody.RemoveChild(bodyHierarquia);

            var bodyArtigo = xmlBody.SelectSingleNode("Artigo");
            xmlBody.RemoveChild(bodyArtigo);

            var xmlIdentifica = xmlBody.SelectSingleNode("Identifica");
            var xmlData = doc.CreateElement("Data");
            xmlBody.InsertAfter(xmlData, xmlIdentifica);

            var xmlTexto = xmlArticle.SelectSingleNode("Texto");            

            return doc.InnerXml;
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
    }
}
