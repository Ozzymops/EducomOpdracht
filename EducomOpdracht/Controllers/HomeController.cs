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
using Chart.Mvc.ComplexChart;
using Chart.Mvc.Extensions;
using System.Data;

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

            // Valideren
            if (!ModelState.IsValid)
            {
                return View(gm);
            }

            // Update data
            API api = new API();
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

                // - Converteer naar Fahrenheit als actief staat
                if (gm.weerstationTemperatuurEenheid)
                {
                    weerstation.TemperatureGc = (weerstation.TemperatureGc * 9) / 5 + 32;
                    weerstation.TemperatureCm = (weerstation.TemperatureCm * 9) / 5 + 32;
                }
            }

            gm.weerstationList = visualWeerstations.Select(x => new SelectListItem() { Text = x.Name.ToString(), Value = x.StationId.ToString() });

            return View(gm);
        }

        public IActionResult Weerbericht()
        {
            return View();
        }

        [HttpPost]
        public JsonResult CreateChart(GraphModel model)
        {
            List<Object> data = new List<object>();

            DataTable dt = new DataTable();
            dt.Columns.Add("Employee", System.Type.GetType("System.String"));
            dt.Columns.Add("Credit", System.Type.GetType("System.Int32"));

            DataRow dr = dt.NewRow();
            dr["Employee"] = "Sam";
            dr["Credit"] = 123;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Employee"] = "Alex";
            dr["Credit"] = 456;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Employee"] = "Michael";
            dr["Credit"] = 789;
            dt.Rows.Add(dr);

            foreach(DataColumn dc in dt.Columns)
            {
                List<object> x = new List<object>();
                x = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();
                data.Add(x);
            }

            return Json(data);
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
