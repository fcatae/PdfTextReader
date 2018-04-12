using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.ParserFrontend
{
    [Route("[controller]")]
    public class DocumentsController : Controller
    {
        public object Index()
        {
            return new { sucesso = true };
        }

        [Route("{name}", Name="Document_Show")]
        public IActionResult Show(string name)
        {
            ViewBag.Name = name;

            return View();
        }
        
        [Route("{name}/{act}", Name="Documents")]
        public object Show(string name, string act)
        {
            return new { docname = name , action = act };
        }
    }
}