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

        /// <summary>
        /// Index, ook wel het weerstation grafiek.
        /// Eerste run (GET) zet gewoon de model op, er wordt nog niks specifieks uitgevoerd.
        /// </summary>
        /// <returns>View met GraphModel</returns>
        public IActionResult Index()
        {
            // Zet API-model op, dat communiceert met de API.
            API api = new API();

            // Zet model op, zodat gegevens al te manipuleren vallen.
            GraphModel gm = new GraphModel();
            
            // Haalt weerstations op uit de API en plaatst ze in een lijst.
            gm.weerstations = api.GetAllWeerstations();

            // Plaatst weerstations van de vorige lijst in een visueel lijst, waar maar één station met een bepaald station ID in kan bestaan.
            // Zo kan een dropdown-list gevormd worden met de weerstations in de database, zonder dat ze er 100x in staan.
            List<Weerstation> visualWeerstations = new List<Weerstation>();
            foreach (Weerstation weerstation in gm.weerstations)
            {
                // Check of een entry al bestaat met dat station ID
                int index = visualWeerstations.FindIndex(o => o.StationId == weerstation.StationId);
                
                // Zo niet, voeg toe aan visueel lijst
                if (index == -1)
                {
                    // Check voor NULL resultaten, zitten om de een of andere reden wel eens erbij, breekt anders de volledige site
                    if (!string.IsNullOrEmpty(weerstation.Name))
                    {
                        visualWeerstations.Add(weerstation);
                    }                
                }
            }

            // Filters van te voren instellen, zodat de checkboxes al aangeklikt zijn
            gm.enableWeerstationTemperatureGc = true;
            gm.enableWeerstationTemperatureCm = true;
            gm.enableWeerstationWindspeed = true;
            gm.enableWeerstationAirPressure = true;
            gm.enableWeerstationHumidity = true;

            // Maakt een nieuwe lijst aan voor geselecteerde weerstation entries, voor de grafiek later
            gm.selectedWeerstations = new List<Weerstation>();

            // Maakt een lijst aan voor de dropdown-list
            gm.weerstationList = visualWeerstations.Select(x => new SelectListItem() { Text = x.Name.ToString(), Value = x.StationId.ToString() });

            // Stuurt View terug met voorgegenereerd model
            return View(gm);
        }

        /// <summary>
        /// Index, ook wel het weerstation grafiek.
        /// Wordt uitgevoerd wanneer een weerstation geselecteerd is en de Submit knop wordt ingedrukt (POST).
        /// Refreshet de weerstation-lijst en populeert een andere lijst dat later gebruikt wordt voor de grafiekgeneratie.
        /// </summary>
        /// <param name="gm">GraphModel, het model met alle data</param>
        /// <returns>View met GraphModel</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(GraphModel gm)
        {
            // Valideert voor errors in het model
            var errors = ModelState.Values.SelectMany(v => v.Errors);

            if (!ModelState.IsValid)
            {
                return View(gm);
            }

            // Updatet de weerstation lijst, stel er zijn ondertussen nieuwe entries tussen gekomen.
            API api = new API();
            gm.weerstations = api.GetAllWeerstations();
            List<Weerstation> visualWeerstations = new List<Weerstation>();
            gm.selectedWeerstations = new List<Weerstation>();

            // Check of er een startdatum is ingevuld, zo niet, stel het in als drie dagen geleden
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

            // Check of er een einddatum is ingevuld, zo niet, stel het in als zeven dagen verder vanaf startdatum
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
                int index = visualWeerstations.FindIndex(o => o.StationId == weerstation.StationId);

                if (index == -1)
                {
                    visualWeerstations.Add(weerstation);
                }

                // Vul de selectedWeerstations lijst met entries die overeen komen met de geselecteerde datumperiode.
                // Alle entries tussen de startdatum en einddatum worden opgeslagen in de lijst, en later gemanipuleerd.
                if (weerstation.StationId.ToString() == gm.selectedListValue)
                {
                    if (weerstation.Date >= gm.startPeriodDate && weerstation.Date <= gm.endPeriodDate)
                    {
                        // Als de Fahrenheit optie aanstaat, converteer de temperaturen naar Fahrenheit.
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

                        // Filters, als een filter uit staat (-> niet tonen dus), wordt de waarde op -999 gezet, wat in de pagina geinterpreteerd wordt als 'niet tonen'.
                        if (!gm.enableWeerstationTemperatureGc)
                        {
                            weerstation.TemperatureGc = -999;
                        }

                        if (!gm.enableWeerstationTemperatureCm)
                        {
                            weerstation.TemperatureCm = -999;
                        }

                        if (!gm.enableWeerstationWindspeed)
                        {
                            weerstation.WindspeedBf = -999;
                            weerstation.WindspeedMs = -999;
                        }

                        if (!gm.enableWeerstationHumidity)
                        {
                            weerstation.Humidity = -999;
                        }

                        if (!gm.enableWeerstationAirPressure)
                        {
                            weerstation.AirPressure = -999;
                        }

                        // Voeg de weerstation toe aan de selected lijst, om te tonen in de grafiek
                        gm.selectedWeerstations.Add(weerstation);
                    }                 
                }
            }

            gm.weerstationList = visualWeerstations.Select(x => new SelectListItem() { Text = x.Name.ToString(), Value = x.StationId.ToString() });

            // Poging tot de datum terug relayen naar de input textboxes, maar helaas werkt dit niet, en ik kom er maar niet achter waarom.
            gm.startPeriod = gm.startPeriodDate.ToString("dd-MM-yyyy HH:mm:ss");
            ViewBag.StartPeriod = gm.startPeriod;

            gm.endPeriod = gm.endPeriodDate.ToString("dd-MM-yyyy HH:mm:ss");
            ViewBag.EndPeriod = gm.endPeriod;

            // Genereer grafiek op basis van modelinformatie, voornamelijk de selected weerstations lijst.
            gm.chartData = CreateStationChart(gm);

            return View(gm);
        }

        /// <summary>
        /// Het weerbericht grafiek.
        /// Eerste run (GET) zet gewoon de model op, er wordt nog niks specifieks uitgevoerd.
        /// </summary>
        /// <returns>View met GraphModel</returns>
        public IActionResult Weerbericht()
        {
            // Zet model op, zodat gegevens al te manipuleren vallen.
            GraphModel gm = new GraphModel();

            // Haalt weerberichten op uit de API en plaatst ze in een lijst.
            gm.selectedWeerberichten = new List<Weerbericht>();

            // Filters van te voren instellen, zodat de checkboxes al aangeklikt zijn
            gm.enableWeerberichtMaxTemp = true;
            gm.enableWeerberichtMinTemp = true;
            gm.enableWeerberichtWindspeed = true;
            gm.enableWeerberichtRainChance = true;
            gm.enableWeerberichtSunChance = true;

            // Stuurt View terug met voorgegenereerd model
            return View(gm);
        }

        /// <summary>
        /// Het weerbericht grafiek.
        /// Wordt uitgevoerd wanneer een datumrange ingevuld is en de Submit knop wordt ingedrukt (POST).
        /// Haalt een lijst met weerberichten binnen de datumrange op en populeert een andere lijst dat later gebruikt wordt voor de grafiekgeneratie.
        /// </summary>
        /// <param name="gm">GraphModel, het model met alle data</param>
        /// <returns>View met GraphModel</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Weerbericht(GraphModel gm)
        {
            // Valideert voor errors in het model
            var errors = ModelState.Values.SelectMany(v => v.Errors);

            if (!ModelState.IsValid)
            {
                return View(gm);
            }

            // Updatet de weerstation lijst, stel er zijn ondertussen nieuwe entries tussen gekomen.
            API api = new API();
            gm.weerberichten = api.GetAllWeerberichten();
            gm.selectedWeerberichten = new List<Weerbericht>();

            // Check of er een einddatum is ingevuld, zo niet, stel het in als zeven dagen verder vanaf startdatum
            if (!string.IsNullOrEmpty(gm.startPeriod))
            {
                try
                {
                    gm.startPeriodDate = DateTime.Parse(gm.startPeriod);
                }
                catch
                {
                    gm.startPeriodDate = DateTime.Now;
                }
            }
            else
            {
                gm.startPeriodDate = DateTime.Now;
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

            // Vul de selectedWeerberichten lijst met entries die overeen komen met de geselecteerde datumperiode.
            // Alle entries tussen de startdatum en einddatum worden opgeslagen in de lijst, en later gemanipuleerd.
            foreach (Weerbericht weerbericht in gm.weerberichten)
            {
                // - Stel selected data in
                if (weerbericht.Date >= gm.startPeriodDate && weerbericht.Date <= gm.endPeriodDate)
                {
                    // Als de Fahrenheit optie aanstaat, converteer de temperaturen naar Fahrenheit.
                    if (gm.weerberichtTemperatuurEenheid)
                    {
                        weerbericht.MaxTemperature = (weerbericht.MaxTemperature * 9) / 5 + 32;
                        weerbericht.MinTemperature = (weerbericht.MinTemperature * 9) / 5 + 32;
                    }

                    // Filters, als een filter uit staat (-> niet tonen dus), wordt de waarde op -999 gezet, wat in de pagina geinterpreteerd wordt als 'niet tonen'.
                    if (!gm.enableWeerberichtMaxTemp)
                    {
                        weerbericht.MaxTemperature = -999;
                    }

                    if (!gm.enableWeerberichtMinTemp)
                    {
                        weerbericht.MinTemperature = -999;
                    }

                    if (!gm.enableWeerberichtWindspeed)
                    {
                        weerbericht.Windspeed = -999;
                    }

                    if (!gm.enableWeerberichtRainChance)
                    {
                        weerbericht.RainChance = -999;
                    }

                    if (!gm.enableWeerberichtSunChance)
                    {
                        weerbericht.SunChance = -999;
                    }

                    // Voeg de weerstation toe aan de selected lijst, om te tonen in de grafiek
                    gm.selectedWeerberichten.Add(weerbericht);
                }
            }

            // Poging tot de datum terug relayen naar de input textboxes, maar helaas werkt dit niet, en ik kom er maar niet achter waarom.
            gm.startPeriod = gm.startPeriodDate.ToString("dd-MM-yyyy HH:mm:ss");
            ViewBag.StartPeriod = gm.startPeriod;

            gm.endPeriod = gm.endPeriodDate.ToString("dd-MM-yyyy HH:mm:ss");
            ViewBag.EndPeriod = gm.endPeriod;

            // Genereer grafiek op basis van modelinformatie, voornamelijk de selected weerstations lijst.
            gm.chartData = CreateBerichtChart(gm);

            return View(gm);
        }

        /// <summary>
        /// Genereert Weerstation grafiek op basis van gegeven data.
        /// </summary>
        /// <param name="model">GraphModel, het model met alle data</param>
        /// <returns>JsonResult met data dat bruikbaar is voor de JavaScript op de site om een grafiek te bouwen</returns>
        public JsonResult CreateStationChart(GraphModel model)
        {
            // Creëer een generieke lijst om de tabel in op te slaan.
            List<Object> data = new List<object>();

            // Creëer een tabel en voeg columns toe met de waarden die we willen tonen op de grafiek.
            DataTable dt = new DataTable();
            dt.Columns.Add("Dag", System.Type.GetType("System.DateTime"));
            dt.Columns.Add("Temperatuur", System.Type.GetType("System.Double"));
            dt.Columns.Add("Temperatuur 10cm", System.Type.GetType("System.Double"));
            dt.Columns.Add("Windsnelheid m/s", System.Type.GetType("System.Double"));
            dt.Columns.Add("Windsnelheid Bf", System.Type.GetType("System.Int32"));
            dt.Columns.Add("Luchtvochtigheid %", System.Type.GetType("System.Int32"));
            dt.Columns.Add("Luchtdruk hPa", System.Type.GetType("System.Double"));

            // Creëer een nieuwe row met data per geselecteerd weerstation.
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

            // Vervorm de gegenereerde tabel entries naar een generieke lijst dat vervolgens wordt toegevoegd naar de generieke lijst van eerder.
            foreach(DataColumn dc in dt.Columns)
            {
                List<object> x = new List<object>();
                x = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();
                data.Add(x);
            }

            // Converteer de generieke lijst naar Json en stuur het terug.
            return Json(data);
        }

        /// <summary>
        /// Genereert Weerbericht grafiek op basis van gegeven data.
        /// </summary>
        /// <param name="model">GraphModel, het model met alle data</param>
        /// <returns>JsonResult met data dat bruikbaar is voor de JavaScript op de site om een grafiek te bouwen</returns>
        public JsonResult CreateBerichtChart(GraphModel model)
        {
            // Creëer een generieke lijst om de tabel in op te slaan.
            List<Object> data = new List<object>();

            // Creëer een tabel en voeg columns toe met de waarden die we willen tonen op de grafiek.
            DataTable dt = new DataTable();
            dt.Columns.Add("Dag", System.Type.GetType("System.DateTime"));
            dt.Columns.Add("Maximum temperatuur", System.Type.GetType("System.Int32"));
            dt.Columns.Add("Minimum temperatuur", System.Type.GetType("System.Int32"));
            dt.Columns.Add("Windsnelheid Bf", System.Type.GetType("System.Int32"));
            dt.Columns.Add("Kans op regen", System.Type.GetType("System.Int32"));
            dt.Columns.Add("Kans op zon", System.Type.GetType("System.Int32"));

            // Creëer een nieuwe row met data per geselecteerd weerbericht.
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

            // Vervorm de gegenereerde tabel entries naar een generieke lijst dat vervolgens wordt toegevoegd naar de generieke lijst van eerder.
            foreach (DataColumn dc in dt.Columns)
            {
                List<object> x = new List<object>();
                x = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();
                data.Add(x);
            }

            // Converteer de generieke lijst naar Json en stuur het terug.
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
