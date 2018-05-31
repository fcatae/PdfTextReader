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
        OutputFiles _outputFiles;

        public ProcessController(AccessManager amgr)
        {
            var vfs = amgr.GetFullAccessFileSystem();

            _pdfHandler = new PdfHandler(vfs);
            _outputFiles = new OutputFiles(vfs);
        }
        
        [HttpPost("{name}/reprocess", Name = "Process_Reprocess")]
        public IActionResult Reprocess(string name)
        {
            var fileList = _pdfHandler.Process(name, "input", "output");

            _outputFiles.Save(name, fileList);

            return this.RedirectToRoute("Document_Show", new { name = name });
        }

        [HttpPost("{name}/delete", Name = "Process_Delete")]
        public IActionResult Delete(string name, [FromServices]DeleteFiles deleteFiles)
        {
            deleteFiles.DestroyAll(name);

            return this.RedirectToPage("/Index");
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

                        var fileList = _pdfHandler.Process(basename, "input", "output");

                        _outputFiles.Save(basename, fileList);

                        return this.RedirectToRoute("Document_Show", new { name = basename });
                    }
                }
            }

            throw new InvalidOperationException("incorrect files");
        }
    }
}