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
        public Task<IActionResult> GetAsync() => ResizeAsync("DO1_2016_01_06", 1, 0, 0, 50, 50);

        // Example:
        // http://localhost/api/images/DO1_2016_01_06/pages/1
        //
        [HttpGet("{document}/pages/{page}")]
        public async Task<IActionResult> GetAsync(string document, int page)
        {
            var img = await _source.GetAsync($"{document}/page_{page}.jpg");

            return new FileStreamResult(img, "image/jpeg");
        }

        // Example:
        // http://localhost/api/images/DO1_2016_01_06/pages/1/resize?x=0&y=0&w=1&h=1
        //
        [HttpGet("{document}/pages/{page}/resize")]
        public async Task<IActionResult> ResizeAsync(string document, int page, [FromQuery]int x, [FromQuery]int y, [FromQuery]int w, [FromQuery]int h)
        {
            var img = await _source.GetAsync($"{document}/page_{page}.jpg");

            float tx = x / 100.0F;
            float ty = y / 100.0F;
            float tw = w / 100.0F;
            float th = h / 100.0F;

            var newimg = ImageProcessing.Crop(img, tx, ty, tw, th);

            return new FileStreamResult(newimg, "image/jpeg");
        }

        public string GetImage(string document, int page)
        {
            return "value";
        }
    }
}
