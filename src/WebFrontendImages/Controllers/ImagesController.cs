using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebFrontendImages.Logic;

namespace WebFrontendImages.Controllers
{
    [Route("api/[controller]")]
    public class ImagesController : Controller
    {
        ImageSource _source;

        public ImagesController(ImageSource source)
        {
            this._source = source;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(string document, int page)
        {
            var img = await _source.GetAsync("DO1_2016_01_06/page_1.jpg");

            return new FileStreamResult(img, "image/jpeg");
        }

        [HttpGet("{document}/pages/{page}")]
        public string GetImage(string document, int page)
        {
            return "value";
        }
    }
}
