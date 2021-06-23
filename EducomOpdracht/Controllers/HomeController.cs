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

            if (!string.IsNullOrEmpty(gm.startPeriod))
            {
                try
                {
                    gm.startPeriodDate = DateTime.Parse(gm.startPeriod);
                }
                catch
                {
                    gm.startPeriodDate = DateTime.Now.AddDays(-3);
                }
            }
            else {
                gm.startPeriodDate = DateTime.Now.AddDays(-3);
            }

            if (!string.IsNullOrEmpty(gm.endPeriod))
            {
                try
                {
                    gm.endPeriodDate = DateTime.Parse(gm.endPeriod);
                }
                catch
                {
                    gm.endPeriodDate = gm.startPeriodDate.AddDays(7);
                }              
            }
            else
            {
                gm.endPeriodDate = gm.startPeriodDate.AddDays(7);
            }

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
                    if (weerstation.Date >= gm.startPeriodDate && weerstation.Date <= gm.endPeriodDate)
                    {
                        gm.selectedWeerstations.Add(weerstation);
                    }                 
                }

                // - Converteer naar Fahrenheit als actief staat
                if (gm.weerstationTemperatuurEenheid)
                {
                    if (weerstation.TemperatureGc != -999)
                    {
                        weerstation.TemperatureGc = (weerstation.TemperatureGc * 9) / 5 + 32;
                    }

                    if (weerstation.TemperatureCm != -999)
                    {
                        weerstation.TemperatureCm = (weerstation.TemperatureCm * 9) / 5 + 32;
                    }
                }
            }

            gm.weerstationList = visualWeerstations.Select(x => new SelectListItem() { Text = x.Name.ToString(), Value = x.StationId.ToString() });

            gm.startPeriod = gm.startPeriodDate.ToString("dd-MM-yyyy HH:mm:ss");
            ViewBag.StartPeriod = gm.startPeriod;

            gm.endPeriod = gm.endPeriodDate.ToString("dd-MM-yyyy HH:mm:ss");
            ViewBag.EndPeriod = gm.endPeriod;

            gm.chartData = CreateStationChart(gm);

            return View(gm);
        }

        public IActionResult Weerbericht()
        {
            GraphModel gm = new GraphModel();

            gm.selectedWeerberichten = new List<Weerbericht>();

            return View(gm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Weerbericht(GraphModel gm)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);

            // Valideren
            if (!ModelState.IsValid)
            {
                return View(gm);
            }

            // Update data
            API api = new API();
            gm.weerberichten = api.GetAllWeerberichten();

            gm.selectedWeerberichten = new List<Weerbericht>();

            if (!string.IsNullOrEmpty(gm.startPeriod))
            {
                try
                {
                    gm.startPeriodDate = DateTime.Parse(gm.startPeriod);
                }
                catch
                {
                    gm.startPeriodDate = DateTime.Now.AddDays(-3);
                }
            }
            else
            {
                gm.startPeriodDate = DateTime.Now.AddDays(-3);
            }

            if (!string.IsNullOrEmpty(gm.endPeriod))
            {
                try
                {
                    gm.endPeriodDate = DateTime.Parse(gm.endPeriod);
                }
                catch
                {
                    gm.endPeriodDate = gm.startPeriodDate.AddDays(7);
                }
            }
            else
            {
                gm.endPeriodDate = gm.startPeriodDate.AddDays(7);
            }

            foreach (Weerbericht weerbericht in gm.weerberichten)
            {
                // - Stel selected data in
                if (weerbericht.Date >= gm.startPeriodDate && weerbericht.Date <= gm.endPeriodDate)
                {
                    gm.selectedWeerberichten.Add(weerbericht);
                }

                // - Converteer naar Fahrenheit als actief staat
                if (gm.weerberichtTemperatuurEenheid)
                {
                    weerbericht.MaxTemperature = (weerbericht.MaxTemperature * 9) / 5 + 32;
                    weerbericht.MinTemperature = (weerbericht.MinTemperature * 9) / 5 + 32;
                }
            }

            gm.startPeriod = gm.startPeriodDate.ToString("dd-MM-yyyy HH:mm:ss");
            ViewBag.StartPeriod = gm.startPeriod;

            gm.endPeriod = gm.endPeriodDate.ToString("dd-MM-yyyy HH:mm:ss");
            ViewBag.EndPeriod = gm.endPeriod;

            gm.chartData = CreateBerichtChart(gm);

            return View(gm);
        }

        public JsonResult CreateStationChart(GraphModel model)
        {
            List<Object> data = new List<object>();

            DataTable dt = new DataTable();
            dt.Columns.Add("Dag", System.Type.GetType("System.DateTime"));
            dt.Columns.Add("Temperatuur", System.Type.GetType("System.Double"));
            dt.Columns.Add("Temperatuur 10cm", System.Type.GetType("System.Double"));
            dt.Columns.Add("Windsnelheid m/s", System.Type.GetType("System.Double"));
            dt.Columns.Add("Windsnelheid Bf", System.Type.GetType("System.Int32"));
            dt.Columns.Add("Luchtvochtigheid %", System.Type.GetType("System.Int32"));
            dt.Columns.Add("Luchtdruk hPa", System.Type.GetType("System.Double"));

            foreach(Weerstation weerstation in model.selectedWeerstations)
            {
                DataRow dr = dt.NewRow();
                dr["Dag"] = weerstation.Date;
                dr["Temperatuur"] = weerstation.TemperatureGc;
                dr["Temperatuur 10cm"] = weerstation.TemperatureCm;
                dr["Windsnelheid m/s"] = weerstation.WindspeedMs;
                dr["Windsnelheid Bf"] = weerstation.WindspeedBf;
                dr["Luchtvochtigheid %"] = weerstation.Humidity;
                dr["Luchtdruk hPa"] = weerstation.AirPressure;
                dt.Rows.Add(dr);
            }

            foreach(DataColumn dc in dt.Columns)
            {
                List<object> x = new List<object>();
                x = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();
                data.Add(x);
            }

            return Json(data);
        }

        public JsonResult CreateBerichtChart(GraphModel model)
        {
            List<Object> data = new List<object>();

            DataTable dt = new DataTable();
            dt.Columns.Add("Dag", System.Type.GetType("System.DateTime"));
            dt.Columns.Add("Maximum temperatuur", System.Type.GetType("System.Int32"));
            dt.Columns.Add("Minimum temperatuur", System.Type.GetType("System.Int32"));
            dt.Columns.Add("Windsnelheid Bf", System.Type.GetType("System.Int32"));
            dt.Columns.Add("Kans op regen", System.Type.GetType("System.Int32"));
            dt.Columns.Add("Kans op zon", System.Type.GetType("System.Int32"));

            foreach (Weerbericht weerbericht in model.selectedWeerberichten)
            {
                DataRow dr = dt.NewRow();
                dr["Dag"] = weerbericht.Date;
                dr["Maximum temperatuur"] = weerbericht.MaxTemperature;
                dr["Minimum temperatuur"] = weerbericht.MinTemperature;
                dr["Windsnelheid Bf"] = weerbericht.Windspeed;
                dr["Kans op regen"] = weerbericht.RainChance;
                dr["Kans op zon"] = weerbericht.SunChance;
                dt.Rows.Add(dr);
            }

            foreach (DataColumn dc in dt.Columns)
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
