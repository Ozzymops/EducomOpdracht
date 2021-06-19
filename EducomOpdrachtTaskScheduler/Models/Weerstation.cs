using System;

namespace EducomOpdrachtTaskScheduler.Models
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

        public Weerstation(long stationId, DateTime date, string region, string name, int temperature, int humidity, int airPressure)
        {
            this.StationId = stationId;
            this.Date = date;
            this.Region = region;
            this.Name = name;
            this.Temperature = temperature;
            this.Humidity = humidity;
            this.AirPressure = airPressure;
        }
    }
}
