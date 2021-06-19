using System;

namespace EducomOpdrachtAPI.Models
{
    public class Weerstation
    {
        /// <summary>
        /// Weerstation vanuit https://data.buienradar.nl/1.1/feed/json opgehaald. Actuele data per weerstation.
        /// </summary>
        public int Id { get; set; }
        public long StationId { get; set; }
        public DateTime Date { get; set; }
        public string Region { get; set; }
        public string Name { get; set; }
        public int Temperature { get; set; }
        public int Humidity { get; set; }
        public int AirPressure { get; set; }
    }
}
