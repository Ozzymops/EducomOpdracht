using System;

namespace EducomOpdrachtAPI.Models
{
    public class Weerbericht
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
    }
}
