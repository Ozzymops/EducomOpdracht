using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EducomOpdracht.Models
{
    public class Weerstation
    {
        // Generieke data
        public int Id { get; set; }
        public long StationId { get; set; }
        public DateTime Date { get; set; }
        public string Region { get; set; }
        public string Name { get; set; }
        // Temperatuur
        public double TemperatureGc { get; set; }   // temperatuur in graden Celsius
        public double TemperatureCm { get; set; }   // temperatuur in graden Celsius op 10 cm hoogte
        // Windsnelheid
        public double WindspeedMs { get; set; }     // windsnelheid in meter per seconde
        public int WindspeedBf { get; set; }        // windsnelheid volgens de schaal van Beaufort: https://www.knmi.nl/kennis-en-datacentrum/uitleg/windschaal-van-beaufort
        // Overig
        public int Humidity { get; set; }           // relatieve luchtvochtigheid in %
        public double AirPressure { get; set; }     // luchtdruk in hectopascal (hPa), ook wel millibar

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
