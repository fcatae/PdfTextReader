using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ParserFrontend.Pages
{
    public class ContactModel : PageModel
    {
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

                Message = file.FileName;
                Console.WriteLine(file.FileName);
            }
        }
    }
}
