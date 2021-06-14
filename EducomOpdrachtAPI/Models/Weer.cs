using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EducomOpdrachtAPI.Models
{
    public class Weer
    {
        /// <summary>
        /// Een entry in de database.
        /// Opgeslagen data bestaat uit datum, temperatuur, luchtvochtigheid en luchtdruk.
        /// </summary>
        public long StationId { get; set; }
        public DateTime Date { get; set; }
        public int Temperature { get; set; }
        public int Humidity { get; set; }
        public int AirPressure { get; set; }
    }
}
