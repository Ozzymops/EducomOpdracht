﻿using System;

namespace EducomOpdrachtAPI.Models
{
    public class Weerstation
    {
        /// <summary>
        /// Weerstation vanuit https://data.buienradar.nl/1.1/feed/json opgehaald. Actuele data per weerstation.
        /// </summary>
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
    }
}
