using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ParserFrontend.Logic;

namespace ParserFrontend.Pages
{
    public class JobsModel : PageModel
    {
        private readonly JobManager _jobMgr;

        public string Message { get; set; }

        public JobsModel(JobManager jobMgr)
        {
            this._jobMgr = jobMgr;
        }

        public void OnGet()
        {
            var queue = _jobMgr.GetAsync().Result;

            Message = String.Join(" | ",queue);
        }
    }
}
