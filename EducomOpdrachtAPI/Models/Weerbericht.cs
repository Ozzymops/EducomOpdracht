using System;

namespace EducomOpdrachtAPI.Models
{
    public class Weerbericht
    {
        /// <summary>
        /// Een entry in de database.
        /// Opgeslagen data bestaat uit datum, temperatuur, luchtvochtigheid en luchtdruk.
        /// </summary>
        public long Id { get; set; }
        public long StationId { get; set; }
        public DateTime Date { get; set; }
        public int Temperature { get; set; }
        public int Humidity { get; set; }
        public int AirPressure { get; set; }
    }
}
