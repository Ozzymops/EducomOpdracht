using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chart.Mvc.ComplexChart;

namespace EducomOpdracht.Models
{
    public class GraphModel
    {
        // Sent
        public long stationId { get; set; }
        public DateTime startPeriod { get; set; }
        public DateTime endPeriod { get; set; }

        // General
        public int graphType = 0;

        // Drop down list
        public string selectedListValue { get; set; }

        // Weerstations
        public List<Weerstation> weerstations { get; set; }
        public List<Weerstation> selectedWeerstations { get; set; }
        public IEnumerable<SelectListItem> weerstationList { get; set; }

        // Weerberichten
        public List<Weerbericht> weerberichten { get; set; }

        // Chart
        public JsonResult chartData { get; set; }
        public bool weerstationWindEenheid { get; set; }
        public bool weerstationTemperatuurEenheid { get; set; }
    }
}
