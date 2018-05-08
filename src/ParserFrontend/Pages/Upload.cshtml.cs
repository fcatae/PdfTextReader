using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ParserFrontend.Logic;

namespace ParserFrontend.Pages
{
    public class UploadModel : PageModel
    {
        AccessManager _amgr;
        public bool HasFullAccess => _amgr.HasFullAccess;

        public UploadModel(AccessManager amgr)
        {
            _amgr = amgr;
        }

        public void OnGet()
        {
        }
    }
}
