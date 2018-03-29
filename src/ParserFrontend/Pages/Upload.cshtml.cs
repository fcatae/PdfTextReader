using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ParserFrontend.Logic;

namespace ParserFrontend.Pages
{
    public class UploadModel : PageModel
    {
        PdfHandler _pdfHandler;

        public UploadModel()
        {
            var vfs = new WebVirtualFS();

            _pdfHandler = new PdfHandler(vfs);
        }

        public string Message { get; set; }

        public void OnGet()
        {
            Message = "Upload page.";
        }

        public void OnPost()
        {
            var files = Request.Form.Files;

            if (files.Count == 1)
            {
                var file = files[0];

                string filename = file.FileName;
                using (var stream = file.OpenReadStream())
                {
                    string basename = _pdfHandler.CreatePdfFile(filename, "bin/input", stream);
                    _pdfHandler.Process(basename, "bin/input", "bin/output");
                }

                Message = file.FileName;
                Console.WriteLine(file.FileName);
            }
        }
    }
}
