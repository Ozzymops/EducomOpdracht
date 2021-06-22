using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EducomOpdracht.Models
{
    public class Weerbericht
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int MaxTemperature { get; set; }
        public int MinTemperature { get; set; }
        public int RainChance { get; set; }
        public int SunChance { get; set; }
    }
}
