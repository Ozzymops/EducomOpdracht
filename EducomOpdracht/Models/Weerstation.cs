using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EducomOpdracht.Models
{
    public class Weerstation
    {
        // ID in de database
        public int Id { get; set; }

        // ID van weerstation (buienradar)
        public long StationId { get; set; }

        // Datum van weerstation-meting
        public DateTime Date { get; set; }

        // Regio van weerstation
        public string Region { get; set; }

        // Naam van weerstation (meestal gewoon Weerstation Regio)
        public string Name { get; set; }

        // Temperatuur van weerstation-meting in graden Celsius
        public double TemperatureGc { get; set; }

        // Temperatuur van weerstation-meting in graden Celsius op 10 cm hoogte
        public double TemperatureCm { get; set; }

        // Windsnelheid van weerstation-meting in meter per seconde
        public double WindspeedMs { get; set; }

        // Windsnelheid van weerstation-meting volgens de schaal van Beaufort: https://www.knmi.nl/kennis-en-datacentrum/uitleg/windschaal-van-beaufort
        public int WindspeedBf { get; set; }

        // Relatieve luchtvochtigheid van weerstation-meting in percentage
        public int Humidity { get; set; }

        // Luchtdruk van weerstation-meting in hectopascal (hPa, ook wel millibar)
        public double AirPressure { get; set; }

        // Constructor voor gemak
        public Weerstation(long stationId, DateTime date, string region, string name, double temperatureGc, double temperatureCm, double windspeedMs, int windspeedBf, int humidity, double airPressure)
        {
            this.StationId = stationId;
            this.Date = date;
            this.Region = region;
            this.Name = name;

            this.TemperatureGc = temperatureGc;
            this.TemperatureCm = temperatureCm;

            this.WindspeedMs = windspeedMs;
            this.WindspeedBf = windspeedBf;

            this.Humidity = humidity;
            this.AirPressure = airPressure;
        }
    }
}
