using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EducomOpdracht.Models
{
    public class GraphModel
    {
        // Verzonden informatie (View -> Model -> Controller)
        // --------------------------------------------------
        // Generiek
        public long stationId { get; set; }
        public string startPeriod { get; set; }
        public string endPeriod { get; set; }
        public DateTime startPeriodDate { get; set; }
        public DateTime endPeriodDate { get; set; }

        // Grafiek
        public bool weerstationWindEenheid { get; set; }
        public bool weerstationTemperatuurEenheid { get; set; }
        public bool weerberichtTemperatuurEenheid { get; set; }
        public bool enableWeerstationTemperatureGc { get; set; }
        public bool enableWeerstationTemperatureCm { get; set; }
        public bool enableWeerstationWindspeed { get; set; }
        public bool enableWeerstationHumidity { get; set; }
        public bool enableWeerstationAirPressure { get; set; }
        public bool enableWeerberichtMaxTemp { get; set; }
        public bool enableWeerberichtMinTemp { get; set; }
        public bool enableWeerberichtWindspeed { get; set; }
        public bool enableWeerberichtRainChance { get; set; }
        public bool enableWeerberichtSunChance { get; set; }

        // Verkregen informatie (Controller -> Model -> View)
        // --------------------------------------------------
        // Generiek
        public string selectedListValue { get; set; }

        // Weerstations
        public List<Weerstation> weerstations { get; set; }
        public List<Weerstation> selectedWeerstations { get; set; }
        public IEnumerable<SelectListItem> weerstationList { get; set; }

        // Weerberichten
        public List<Weerbericht> weerberichten { get; set; }
        public List<Weerbericht> selectedWeerberichten { get; set; }
        public IEnumerable<SelectListItem> weerberichtList { get; set; }

        // Grafiek
        public JsonResult chartData { get; set; }
    }
}
