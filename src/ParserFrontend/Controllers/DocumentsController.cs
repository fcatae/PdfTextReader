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
        PdfHandler _pdfHandler;

        public DocumentsController()
        {
            var vfs = new WebVirtualFS();

            _pdfHandler = new PdfHandler(vfs);
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
            var output = new ParserFrontend.Logic.OutputFiles();

            // check if the file was processed
            // ...
            // ...

            return new FileStreamResult(output.GetOutputFile(name), "application/pdf");
        }

        [Route("{name}/{act}")]
        public object Show(string name, string act)
        {
            return new { docname = name , action = act };
        }
    }
}