using EducomOpdracht.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EducomOpdracht.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            API api = new API();
            GraphModel gm = new GraphModel();
            
            // First time get data
            gm.weerstations = api.GetAllWeerstations();

            List<Weerstation> visualWeerstations = new List<Weerstation>();
            foreach (Weerstation weerstation in gm.weerstations)
            {
                // Check of een entry al bestaat met dat station ID
                int index = visualWeerstations.FindIndex(o => o.StationId == weerstation.StationId);
                
                // Zo niet, voeg toe aan visueel lijst
                if (index == -1)
                {
                    visualWeerstations.Add(weerstation);
                }
            }

            gm.selectedWeerstations = new List<Weerstation>();
            gm.weerstationList = visualWeerstations.Select(x => new SelectListItem() { Text = x.Name.ToString(), Value = x.StationId.ToString() });

            return View(gm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(GraphModel gm)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);

            if (ModelState.IsValid)
            {

            }

            API api = new API();

            // Update data
            gm.weerstations = api.GetAllWeerstations();

            List<Weerstation> visualWeerstations = new List<Weerstation>();
            gm.selectedWeerstations = new List<Weerstation>();

            foreach (Weerstation weerstation in gm.weerstations)
            {
                // Check of een entry al bestaat met dat station ID
                int index = visualWeerstations.FindIndex(o => o.StationId == weerstation.StationId);

                // Zo niet, voeg toe aan visueel lijst
                if (index == -1)
                {
                    visualWeerstations.Add(weerstation);
                }

                // - Stel selected data in
                if (weerstation.StationId.ToString() == gm.selectedListValue)
                {
                    gm.selectedWeerstations.Add(weerstation);
                }
            }

            gm.weerstationList = visualWeerstations.Select(x => new SelectListItem() { Text = x.Name.ToString(), Value = x.StationId.ToString() });

            return View(gm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
