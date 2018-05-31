using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ParserFrontend.Logic;

namespace ParserFrontend.Controllers
{
    [Route("list")]
    public class ListController : Controller
    {
        private readonly InputFiles _inputFiles;

        public ListController(AccessManager amgr)
        {
            var vfs = amgr.GetReadOnlyFileSystem();

            this._inputFiles = new InputFiles(vfs);
        }

        [HttpGet("", Name ="List_Index")]
        public IActionResult Index()
        {
            ViewBag.DO1 = EnumAvailableFiles(d => d.StartsWith("DO1_"));
            ViewBag.DO2 = EnumAvailableFiles(d => d.StartsWith("DO2_"));
            ViewBag.DO3 = EnumAvailableFiles(d => d.StartsWith("DO3_"));
            ViewBag.Outros = EnumAvailableFiles(d => (!d.StartsWith("DO1_")) && (!d.StartsWith("DO2_")) && (!d.StartsWith("DO3_")));

            return View();
        }
        
        string[] EnumAvailableFiles(Func<string,bool> filter)
        {
            return _inputFiles
                    .List()
                    .Where(filter)
                    .ToArray();
        }
    }
}