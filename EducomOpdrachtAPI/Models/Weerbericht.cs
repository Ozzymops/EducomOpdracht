using System;

namespace EducomOpdrachtAPI.Models
{
    public class Weerbericht
    {
        // ID in de database
        public int Id { get; set; }

        // Datum van weerbericht
        public DateTime Date { get; set; }

        // Maximum temperatuur van de dag in graden Celsius
        public int MaxTemperature { get; set; }

        // Minimum temperatuur van de dag in graden Celsius
        public int MinTemperature { get; set; }

        // Windsnelheid gedurende de dag volgens de schaal van Beaufort: https://www.knmi.nl/kennis-en-datacentrum/uitleg/windschaal-van-beaufort
        public int Windspeed { get; set; }

        // Kans op regen gedurende de dag in percentage
        public int RainChance { get; set; }

        // Kans op zon gedurende de dag in percentage
        public int SunChance { get; set; }

        // Standaard constructor zonder parameters
        public Weerbericht()
        {

        }

        // Constructor voor gemak
        public Weerbericht(DateTime date, int maxTemp, int minTemp, int windspeed, int rainChance, int sunChance)
        {
            this.Date = date;
            this.MaxTemperature = maxTemp;
            this.MinTemperature = minTemp;
            this.Windspeed = windspeed;
            this.RainChance = rainChance;
            this.SunChance = sunChance;
        }
    }
}
