using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducomOpdrachtTaskScheduler.Models
{
    class Weerbericht
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

        public Weerbericht(DateTime date, long stationId, int temperature, int humidity, int airPressure)
        {
            this.Date = date;
            this.StationId = stationId;
            this.Temperature = temperature;
            this.Humidity = humidity;
            this.AirPressure = airPressure;
        }

        public Weerbericht(DateTime date, int maxTemperature, int minTemperature, int rainChance, int sunChance)
        {
            this.StationId = -1;
            this.Date = date;
            this.MaxTemperature = maxTemperature;
            this.MinTemperature = minTemperature;
            this.RainChance = rainChance;
            this.SunChance = sunChance;
        }
    }
}
