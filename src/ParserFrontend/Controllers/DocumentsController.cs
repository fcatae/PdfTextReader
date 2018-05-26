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

        [Route("{name}/logs")]
        public IActionResult Log(string name)
        {
            ViewBag.Name = name;
            return View();
        }
    }
}