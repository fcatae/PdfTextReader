using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ParserFrontend.Logic;

namespace ParserFrontend.Controllers
{
    [Route("[controller]")]
    public class DocumentsController : Controller
    {
        OutputFiles _outputFiles;

        public DocumentsController(AccessManager amgr)
        {
            var vfs = amgr.GetReadOnlyFileSystem();

            _outputFiles = new OutputFiles(vfs);
        }

        public object Index()
        {
            return new { sucesso = true };
        }

        [Route("{name}", Name="Document_Show")]
        public IActionResult Show(string name)
        {
            ViewBag.Name = name;

            return View();
        }

        [Route("{name}/output", Name = "Document_Output")]
        public IActionResult ShowOutput(string name)
        {
            //var output = new ParserFrontend.Logic.OutputFiles();

            // check if the file was processed
            // ...
            // ...

            return new FileStreamResult(_outputFiles.GetOutputFile(name), "application/pdf");
        }

        [Route("{name}/tree", Name = "Document_OutputTree")]
        public string ShowTree(string name)
        {
            return _outputFiles.GetOutputTree(name).ToString();
        }

        [Route("{name}/articles/{id}")]
        public string ShowDetailedArticle(string name, int id)
        {
            string artigo = _outputFiles.GetOutputArtigo(name, id).ToString();

            return PdfTextReader.ExampleStages.ConvertGN(name, id.ToString(), artigo);
        }

        [Route("{name}/art/{id}")]
        public string ShowArticle(string name, int id)
        {
            string artigo = _outputFiles.GetOutputArtigo(name, id).ToString();
            return artigo;
        }

        [Route("{name}/{act}")]
        public object Show(string name, string act)
        {
            return new { docname = name, action = act };
        }
        
        [Route("{name}/articles/{id}/html")]
        public IActionResult ShowHtmlArticle(string name, int id)
        {
            string artigo = _outputFiles.GetOutputArtigo(name, id).ToString();

            string xml = PdfTextReader.ExampleStages.ConvertGN(name, id.ToString(), artigo);

            var doc = new System.Xml.XmlDocument();
            doc.LoadXml(xml);
            var texto = doc.SelectSingleNode("xml/article/body/Texto").InnerText;

            return Content(texto, "text/html");
        }
    }
}