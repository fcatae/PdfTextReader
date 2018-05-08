using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ParserFrontend.Logic;

namespace ParserFrontend.Pages
{
    public class IndexModel : PageModel
    {
        public IEnumerable<string> CurrentFiles { get; private set; }

        IVirtualFS2 _vfs;

        public IndexModel(AccessManager amgr)
        {
            _vfs = amgr.GetReadOnlyFileSystem();
        }

        public void OnGet()
        {
            var inputf = new Logic.InputFiles(_vfs);

            CurrentFiles = inputf.List();
        }
    }
}
