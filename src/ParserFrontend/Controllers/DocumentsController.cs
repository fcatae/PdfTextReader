using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ParserFrontend.Logic;

namespace ParserFrontend.Controllers
{
    [Route("[controller]")]
    public class DocumentsController : Controller
    {
        public class Config
        {
            public float ImageRatio { get; set; }
        }

        OutputFiles _outputFiles;
        float _imageRatio;

        public DocumentsController(AccessManager amgr, IOptions<Config> options)
        {
            var vfs = amgr.GetReadOnlyFileSystem();

            _outputFiles = new OutputFiles(vfs);

            _imageRatio = options.Value.ImageRatio;
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
        public IActionResult ShowArticle(string name, int id)
        {
            return RedirectToRoute("Document_ArticleDefault");
        }

        [Route("{name}/articles/{id}/default", Name="Document_ArticleDefault")]
        public IActionResult ShowArticleDefault(string name, int id)
        {
            return ShowArticleHtml(name, id);
        }

        [Route("{name}/articles/{id}/text")]
        public string ShowArticleText(string name, int id)
        {
            string artigo = _outputFiles.GetOutputArtigo(name, id).ToString();
            return artigo;
        }

        [Route("{name}/articles/{id}/gn4")]
        public IActionResult ShowArticleGN4(string name, int id)
        {
            string artigo = _outputFiles.GetOutputArtigo(name, id).ToString();

            return Content( PdfTextReader.ExampleStages.ConvertGN(name, id.ToString(), artigo), "text/xml" );
        }

        [Route("{name}/articles/{id}.html")]
        public IActionResult ShowArticleHtml(string name, int id)
        {
            string artigo = _outputFiles.GetOutputArtigo(name, id).ToString();

            string xml = PdfTextReader.ExampleStages.ConvertGN(name, id.ToString(), artigo);

            var doc = new System.Xml.XmlDocument();
            doc.LoadXml(xml);
            var texto = doc.SelectSingleNode("xml/article/body/Texto").InnerText;
            var pdfPage = doc.SelectSingleNode("xml/article/@pdfPage")?.InnerText;

            var html = $"<html><head><link rel='stylesheet' type='text/css' href='/css/gn.css'><meta charset='UTF-8'><title>{name}</title></head></html><body>{texto}</body>";

            ViewBag.Name = name;
            ViewBag.Html = texto;
            ViewBag.ImageRatio = _imageRatio;
            ViewBag.PdfPage = pdfPage ?? "";

            //return Content(html, "text/html");

            return View("ShowArticleHtml");
        }

        [Route("{name}/logs")]
        public IActionResult Log(string name)
        {
            ViewBag.Name = name;
            return View();
        }
    }
}