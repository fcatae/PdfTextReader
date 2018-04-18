using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ParserFrontend.Logic;

namespace ParserFrontend.Controllers
{
    [Route("[controller]")]
    public class ProcessController : Controller
    {
        PdfHandler _pdfHandler;

        public ProcessController()
        {
            var vfs = new WebVirtualFS();

            _pdfHandler = new PdfHandler(vfs);
        }
        
        [HttpPost("{name}/reprocess", Name = "Process_Reprocess")]
        public IActionResult Reprocess(string name)
        {
            _pdfHandler.Process(name, "input", "output");
            
            return this.RedirectToRoute("Document_Show", new { name = name });
        }

        [HttpPost("", Name = "Process_Upload")]
        public IActionResult Upload()
        {
            var files = Request.Form.Files;

            if (files.Count == 1)
            {
                var file = files[0];

                string filename = file.FileName;
                long length = file.Length;

                if (filename != "" && length > 0)
                {
                    using (var stream = file.OpenReadStream())
                    {
                        string basename = _pdfHandler.CreatePdfFile(filename, "input", stream);

                        _pdfHandler.Process(basename, "input", "output");

                        return this.RedirectToRoute("Document_Show", new { name = basename });
                    }
                }
            }

            throw new InvalidOperationException("incorrect files");
        }
    }
}