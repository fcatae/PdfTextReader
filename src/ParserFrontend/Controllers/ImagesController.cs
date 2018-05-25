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
    public class ImagesController : Controller
    {
        public class Config
        {
            public string RedirectSite { get; set; }
        }

        string _site;

        public ImagesController(IOptions<Config> options)
        {
            _site = options.Value.RedirectSite;
        }

        [Route("{name}/{*image}")]
        public IActionResult Get(string name, string image)
        {
            string url = $"{_site}/api/images/{name}/parser/{image}";

            return Redirect(url);
        }
    }
}