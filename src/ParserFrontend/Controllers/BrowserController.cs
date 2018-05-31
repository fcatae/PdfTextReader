using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ParserFrontend.Logic;

namespace ParserFrontend.Controllers
{
    [Route("browser")]
    public class BrowserController : Controller
    {
        private readonly InputFiles _inputFiles;

        public BrowserController(AccessManager amgr)
        {
            var vfs = amgr.GetReadOnlyFileSystem();

            this._inputFiles = new InputFiles(vfs);
        }

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

            var availableDates = EnumAvailableDates(year, tipo);

            ViewBag.AvailableDatesHtml = String.Join(",", availableDates.Select(d => $"{{ date: '{d}', count: 1 }}"));

            return View();
        }

        string[] EnumAvailableDates(int year, string tipo)
        {
            return _inputFiles
                    .List()
                    .Where(n => n.StartsWith($"{tipo}_{year}"))
                    .Select(n => n.Substring(tipo.Length + 1, 4 + 1 + 2 + 1 + 2).Replace("_", "-"))
                    .ToArray();
        }
    }
}