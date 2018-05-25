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
        public Task<IActionResult> GetAsync() => GetAsync("DO1_2016_01_06", 1);

        // Example:
        // http://localhost/api/images/DO1_2016_01_06/pages/1
        //
        [HttpGet("{document}/pages/{page}")]
        public async Task<IActionResult> GetAsync(string document, int page)
        {
            var img = await _source.GetAsync($"{document}/page_{page}.jpg");

            return new FileStreamResult(img, "image/jpeg");
        }

        public string GetImage(string document, int page)
        {
            return "value";
        }
    }
}
