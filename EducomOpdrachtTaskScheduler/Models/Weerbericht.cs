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
        /// Een entry in de database. Toekomstige data uit 5-daags weervoorspelling.
        /// </summary>
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int MaxTemperature { get; set; }     // maximum temperatuur in graden Celsius
        public int MinTemperature { get; set; }     // minimum temperatuur in graden Celsius
        public int Windspeed { get; set; }          // windsnelheid volgens de schaal van Beaufort: https://www.knmi.nl/kennis-en-datacentrum/uitleg/windschaal-van-beaufort
        public int RainChance { get; set; }         // kans op regen in %
        public int SunChance { get; set; }          // kans op zon in %

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
