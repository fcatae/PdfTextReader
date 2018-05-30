using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ParserFrontend.Logic;

namespace ParserFrontend.Controllers
{
    [Route("documents/{name}/articles")]
    public class ArticlesController : Controller
    {
        public class Config
        {
            public float ImageRatio { get; set; }
        }

        OutputFiles _outputFiles;
        float _imageRatio;
        private PrettyTextFile _prettifier;

        public ArticlesController(AccessManager amgr, PrettyTextFile prettifier, IOptions<Config> options)
        {
            var vfs = amgr.GetReadOnlyFileSystem();

            _outputFiles = new OutputFiles(vfs);

            _imageRatio = options.Value.ImageRatio;

            this._prettifier = prettifier;
        }

        public object Index()
        {
            return new { sucesso = true };
        }

        [Route("{id}")]
        public IActionResult Show(string name, int id)
        {
            return RedirectToRoute("Articles_Default");
        }

        [Route("{id}/prev", Name = "Articles_Previous")]
        public IActionResult MovePrev(string name, int id)
        {
            int prevId = id - 1;

            if (prevId < 0)
                return RedirectToRoute("Document_Show", new { Name = name});

            return RedirectToRoute("Articles_Default", new { Name = name, id = prevId });
        }

        [Route("{id}/next", Name ="Articles_Next")]
        public IActionResult MoveNext(string name, int id)
        {
            int nextId = id + 1;

            if(!_outputFiles.ExistsArtigo(name, nextId))
                return RedirectToRoute("Document_Show", new { Name = name });

            return RedirectToRoute("Articles_Default", new { Name = name, id = nextId });
        }

        [Route("{id}/default", Name= "Articles_Default")]
        public IActionResult ShowDefault(string name, int id)
        {
            return ShowHtml(name, id);
        }

        [Route("{id}/text")]
        public string ShowText(string name, int id)
        {
            string artigo = _outputFiles.GetOutputArtigo(name, id).ToString();
            return artigo;
        }

        [Route("{id}/formatted")]
        public string ShowFormattedText(string name, int id)
        {
            string artigo = _outputFiles.GetOutputArtigo(name, id).ToString();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(artigo);

            string titulo = doc.SelectSingleNode("article/body/Identifica").InnerText;
            string texto = doc.SelectSingleNode("article/body/Artigo").InnerText;

            _prettifier.SetWidth(texto);

            string pretty = _prettifier.Process(texto);

            return $"{titulo}{"".PadRight(titulo.Length,'=')}\r\n\r\n{pretty}";
        }

        [Route("{id}/gn4")]
        public IActionResult ShowGN4(string name, int id)
        {
            string artigo = _outputFiles.GetOutputArtigo(name, id).ToString();

            return Content( PdfTextReader.ExampleStages.ConvertGN(name, id.ToString(), artigo), "text/xml" );
        }

        [Route("{id}.html")]
        public IActionResult ShowHtml(string name, int id)
        {
            string artigo = _outputFiles.GetOutputArtigo(name, id).ToString();

            string xml = PdfTextReader.ExampleStages.ConvertGN(name, id.ToString(), artigo);

            var doc = new System.Xml.XmlDocument();
            doc.LoadXml(xml);
            var texto = doc.SelectSingleNode("xml/article/body/Texto").InnerText;
            var pdfPage = doc.SelectSingleNode("xml/article/@pdfPage")?.InnerText;
            var artCategory = doc.SelectSingleNode("xml/article/@artCategory")?.InnerText;

            var html = $"<html><head><link rel='stylesheet' type='text/css' href='/css/gn.css'><meta charset='UTF-8'><title>{name}</title></head></html><body>{texto}</body>";

            ViewBag.Name = name;
            ViewBag.Id = id;
            ViewBag.Html = texto;
            ViewBag.ImageRatio = _imageRatio;
            ViewBag.PdfPage = pdfPage ?? "";
            ViewBag.ArtCategory = artCategory.Split('/');
            return View("ShowHtml");
        }
    }
}