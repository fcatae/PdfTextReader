using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ParserFrontend.Pages
{
    public class IndexModel : PageModel
    {
        public IEnumerable<string> CurrentFiles { get; private set; }
        public void OnGet()
        {
            var inputf = new Logic.InputFiles(new WebVirtualFS());

            CurrentFiles = inputf.List();
        }
    }
}
