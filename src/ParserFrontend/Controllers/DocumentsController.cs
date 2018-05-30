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
        private readonly DownloadFolder _downloader;

        public DocumentsController(AccessManager amgr, DownloadFolder downloader, IOptions<Config> options)
        {
            var vfs = amgr.GetReadOnlyFileSystem();

            this._outputFiles = new OutputFiles(vfs);
            this._imageRatio = options.Value.ImageRatio;
            this._downloader = downloader;
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

        [Route("{name}/output/{logfile}")]
        public IActionResult ShowLogOutput(string name, string logfile)
        {
            return new FileStreamResult(_outputFiles.GetLogFile(name, logfile), "application/pdf");
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
        public object LogfileList(string name)
        {
            var fileList = _outputFiles.Load(name);

            return fileList;
        }

        [Route("{name}/logs/{logname}")]
        public IActionResult Log(string name, string logname)
        {
            ViewBag.Name = name;
            ViewBag.LogName = logname;
            return View();
        }

        [HttpGet("{name}/zip/artigosGN4")]
        public IActionResult DownloadArtigosGN4(string name)
        {
            var stream = _downloader.Download($"output/{name}/artigosGN4");

            return new FileStreamResult(stream, "application/zip");
        }

        [HttpGet("files/{name}.zip")]
        public IActionResult FileDownloadArtigosGN4(string name)
        {
            var stream = _downloader.Download($"output/{name}/artigosGN4");

            return new FileStreamResult(stream, "application/zip");
        }
    }
}