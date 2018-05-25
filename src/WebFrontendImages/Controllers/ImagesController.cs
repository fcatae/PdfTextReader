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

        // Example:
        // http://localhost/api/images/DO1_2016_01_06/pages/1
        //
        [HttpGet("{document}/pages/{page}")]
        [ResponseCache(Duration = 80000)]
        public async Task<IActionResult> GetAsync(string document, int page)
        {
            var img = await _source.GetAsync($"{document}/page_{page}.jpg");

            return new FileStreamResult(img, "image/jpeg");
        }

        // Example:
        // http://localhost/api/images/DO1_2016_01_06/pages/1/resize?x=0&y=0&w=1&h=1
        //
        [HttpGet("{document}/pages/{page}/resize")]
        public Task<IActionResult> ResizeAsync(string document, int page, [FromQuery]int x, [FromQuery]int y, [FromQuery]int w, [FromQuery]int h)
        {
            float tx = x / 100.0F;
            float ty = y / 100.0F;
            float tw = w / 100.0F;
            float th = h / 100.0F;

            return InternalResizeAsync(document, page, tx, ty, tw, th);
        }

        // TABLE(page=2,49.018,776.8651,390.361,830.812)
        [HttpGet("{document}/parser/TABLE(page={page},{docx1},{docy1},{docx2},{docy2})")]
        [ResponseCache(Duration=80000)]
        public Task<IActionResult> LegacyResizeTableAsync(string document, int page, float docx1, float docy1, float docx2, float docy2)
        {
            const float A4_height = 907F;
            const float A4_width = 822F;

            float tx = docx1 / A4_width;
            float ty = (A4_height - docy2) / A4_height;
            float tw = (docx2 - docx1 + 2) / A4_width;
            float th = (docy2 - docy1 + 2) / A4_height;

            return InternalResizeAsync(document, page, tx, ty, tw, th);
        }

        // IMG(page=2,49.018,776.8651,390.361,830.812)
        [HttpGet("{document}/parser/IMG(page={page},{docx1},{docy1},{docx2},{docy2})")]
        [ResponseCache(Duration = 80000)]
        public Task<IActionResult> LegacyResizeImageAsync(string document, int page, float docx1, float docy1, float docx2, float docy2)
        {
            const float A4_height = 907F;
            const float A4_width = 822F;

            float tx = docx1 / A4_width;
            float ty = (A4_height - docy2) / A4_height;
            float tw = (docx2 - docx1 + 2) / A4_width;
            float th = (docy2 - docy1 + 2) / A4_height;

            return InternalResizeAsync(document, page, tx, ty, tw, th);
        }

        private async Task<IActionResult> InternalResizeAsync(string document, int page, float fx, float fy, float fw, float fh)
        {
            var img = await _source.GetAsync($"{document}/page_{page}.jpg");

            var newimg = ImageProcessing.Crop(img, fx, fy, fw, fh);

            return new FileStreamResult(newimg, "image/jpeg");
        }
    }
}
