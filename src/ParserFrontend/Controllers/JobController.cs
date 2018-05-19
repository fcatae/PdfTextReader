using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ParserFrontend.Logic;

namespace ParserFrontend.Controllers
{
    [Route("[controller]")]
    public class JobController : Controller
    {
        private JobProcess _job;

        public JobController(JobProcess job)
        {
            this._job = job;
        }
        
        [Route("{*name}")]
        public bool Start(string name)
        {
            _job.Process(name);

            return true;
        }
    }
}