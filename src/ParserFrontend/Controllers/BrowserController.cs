using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ParserFrontend.Controllers
{
    [Route("browser")]
    public class BrowserController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("{tipo}/{year}", Name="Browser_YearTipo")]
        public IActionResult Year(int year, string tipo)
        {
            var dict = new Dictionary<string, string> { { "DO1", "Seção 1" }, { "DO2", "Seção 2" }, { "DO3", "Seção 3" } };

            ViewBag.Name = dict[tipo];
            ViewBag.Year = year;

            return View();
        }
    }
}