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
        public int MaxTemperature { get; set; }
        public int MinTemperature { get; set; }
        public int RainChance { get; set; }
        public int SunChance { get; set; }

        public Weerbericht(DateTime date, int maxTemp, int minTemp, int rainChance, int sunChance)
        {
            this.Date = date;
            this.MaxTemperature = maxTemp;
            this.MinTemperature = minTemp;
            this.RainChance = rainChance;
            this.SunChance = sunChance;
        }
    }
}
