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
        public DateTime Date { get; set; }

        // Weerstation-linked
        public long StationId { get; set; }
        public int Temperature { get; set; }
        public int Humidity { get; set; }
        public int AirPressure { get; set; }

        // Meerdaagse voorspelling
        public int MaxTemperature { get; set; }
        public int MinTemperature { get; set; }
        public int RainChance { get; set; }
        public int SunChance { get; set; }
    }
}
