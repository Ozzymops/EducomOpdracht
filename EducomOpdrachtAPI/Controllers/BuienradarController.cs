using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EducomOpdrachtAPI.Controllers
{
    public class BuienradarController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
